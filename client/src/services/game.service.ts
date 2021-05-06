import { hexlify } from 'ethers/lib/utils';
import { LabRatsToken } from '../contracts/LabRatsToken';
import LabRatsTokenABI from '../contracts/labRatsToken.json';
import {
  GetPersistentGameDataByIDResponse,
  MultiplayerGamesManager,
} from '../contracts/MultiplayerGamesManager';
import MultiplayerGamesManagerABI from '../contracts/multiplayerGamesManager.json';
import { Bet } from '../model/bet';
import { RoundResult } from '../model/roundResult';
import {
  addBet,
  clearBets,
  setCanPlay,
  setGameError,
  setResult,
  setRound,
} from '../store/actions/game.actions';
import store from '../store/store';
import { ethers } from './ether.service';
import { EndGameRound } from '../events/endGameRound';
import { BetEvent } from '../events/betEvent';
import { PersistentDataEvent } from '../events/persistentDataEvent';
import { ErrorCode } from '../model/errorCodes';
import {
  freezeDisplayBalance,
  unFreezeDisplayBalance,
} from '../store/actions/user.actions';
import { Round } from '../model/round';
import { NoMoreBets } from '../events/noMoreBets';
import { StartGameRound } from '../events/startGameRound';
import { TransferAndCallType } from '../model/transferAndCallType';
class GameService {
  private GAME_MANAGER_ADDRESS = '0x832B7d868C45a53e9690ffc12527391098bBd0dD';
  private TOKEN_ADDRESS = '0x11160251d4283A48B7A8808aa0ED8EA5349B56e2';

  public handleBalanceFreeze = (frozen: boolean) => {
    if (frozen) {
      store.dispatch(freezeDisplayBalance());
    } else {
      store.dispatch(unFreezeDisplayBalance());
    }
  };

  public handlePlay = async (stake: number, action: number) => {
    const state = store.getState();
    const bet: Bet = {
      roundId: state.game.round?.id as string,
      address: state.user.address as string,
      amount: stake,
      data: hexlify(action),
      confirmed: false,
    };

    if (!bet.roundId) {
      store.dispatch(
        setGameError({
          code: ErrorCode.GENERAL_BET_ERROR,
          msg: 'Invalid round ID',
        })
      );
      throw new Error('Error placing bet. Round id not found');
    }

    const encoded = ethers.encode(
      [TransferAndCallType],
      [
        {
          roundID: bet.roundId,
          bets: [
            {
              playerAddress: bet.address,
              betAmount: hexlify(bet.amount),
              betData: bet.data,
            },
          ],
        },
      ]
    );

    const contract = await ethers.getContract<LabRatsToken>(
      LabRatsTokenABI,
      this.TOKEN_ADDRESS
    );

    try {
      const transactionResponse = await contract.transferAndCall(
        this.GAME_MANAGER_ADDRESS,
        bet.amount,
        encoded
      );

      // messageService.broadcastBet(bet);

      const receipt = await transactionResponse.wait(1);
      console.log('handlePlay receipt: ', receipt);
    } catch (error) {
      console.error(error);
      store.dispatch(
        setGameError({
          code: ErrorCode.GENERAL_BET_ERROR,
          msg: 'Error placing bet',
        })
      );
    }
  };

  public async subscribeToContractEvents() {
    const contract = await ethers.getContract<MultiplayerGamesManager>(
      MultiplayerGamesManagerABI,
      this.GAME_MANAGER_ADDRESS
    );

    (contract as any).on('StartGameRound', (...args: any[]) => {
      const round: Round = {
        id: args[StartGameRound.ROUND_ID],
        //TODO: remove redundant fields
        block: 0,
        time: 0,
        timeToNextRound: 0,
      };

      store.dispatch(setRound(round));
    });

    (contract as any).on('Bet', (...args: any[]) => {
      const bet: Bet = {
        roundId: args[BetEvent.ROUND_ID],
        address: args[BetEvent.DATA]['playerAddress'],
        amount: args[BetEvent.DATA]['betAmount'],
        data: args[BetEvent.DATA]['betData'],
        confirmed: true,
      };

      store.dispatch(addBet(bet));
    });

    (contract as any).on('NoMoreBets', (...args: any[]) => {
      const state = store.getState();
      if (args[NoMoreBets.ROUND_ID] === state.game.round?.id) {
        store.dispatch(setCanPlay(false));
      }
    });

    (contract as any).on('EndGameRound', async (...args: any[]) => {
      const persistentData: GetPersistentGameDataByIDResponse | null = await contract
        .getPersistentGameDataByID(args[EndGameRound.PERSISTENT_GAME_DATA_ID])
        .catch((error) => {
          console.error(error);
          store.dispatch(
            setGameError({
              code: ErrorCode.JSON_RPC_READ_ERROR,
              msg: 'Error reading persistant game data',
            })
          );
          return null;
        });

      const result: RoundResult = {
        id: args[EndGameRound.ROUND_ID],
        playerAddresses: args[EndGameRound.PLAYER_ADDRESSES],
        winAmounts: args[EndGameRound.WIN_AMOUNTS],
        result: args[EndGameRound.GAME_RESULT],
        history: args[EndGameRound.HISTORY],
        potWinLoss: args[EndGameRound.POT_WIN_LOSS],
        entropyReveal: args[EndGameRound.ENTROPY_REVEAL],
        persistentGameData: !!persistentData
          ? {
              id: args[EndGameRound.PERSISTENT_GAME_DATA_ID],
              data: persistentData[PersistentDataEvent.GAME_DATA],
              pot: persistentData[PersistentDataEvent.POT_VALUE].toNumber(),
            }
          : null,
      };

      if (!!result) {
        store.dispatch(setResult(result));
        store.dispatch(clearBets());
      }
    });
  }
}

export const gameService = new GameService();
