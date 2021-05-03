import { hexlify } from 'ethers/lib/utils';
import { LabRatsToken } from '../contracts/LabRatsToken';
import LabRatsTokenABI from '../contracts/labRatsToken.json';
import { GetPersistentGameDataByIDResponse, MultiplayerGamesManager } from '../contracts/MultiplayerGamesManager';
import MultiplayerGamesManagerABI from '../contracts/multiplayerGamesManager.json';
import { Bet } from '../model/bet';
import { BlockHeader } from '../model/blockHeader';
import { RoundResult } from '../model/roundResult';
import { addBet, clearBets, setGameError, setResult } from '../store/actions/game.actions';
import store from '../store/store';
import { ethers } from './ether.service';
import { messageService } from './message.service';
import { isContractAddressInBloom, isInBloom } from 'ethereum-bloom-filters';
import { Event } from '@ethersproject/contracts';
import { EndGameRound } from '../events/endGameRound';
import { BetEvent } from '../events/betEvent';
import { PersistentDataEvent } from '../events/persistentDataEvent';
import { ErrorCode } from '../model/errorCodes';
import { setNetworkError } from '../store/actions/network.actions';
import { freezeDisplayBalance, unFreezeDisplayBalance } from '../store/actions/user.actions';
class GameService {
  private GAME_MANAGER_ADDRESS = '0x42f9A9bDe939E9f0e082a801D7245005a1066681';
  private TOKEN_ADDRESS = '0x11160251d4283A48B7A8808aa0ED8EA5349B56e2';

  private debugEncodeBet(bet: Bet): string {

    const leftPadValue = (value: string, digits=64) => {
      return Array(Math.max(digits - value.length + 1, 0)).join('0') + value;
    }


    const uint256Type = '0000000000000000000000000000000000000000000000000000000000000020';
    const roundId = bet.roundId.substring(2);
    const arrayType = '0000000000000000000000000000000000000000000000000000000000000040';
    const storageType = '0000000000000000000000000000000000000000000000000000000000000060';
    const arrayLength = '0000000000000000000000000000000000000000000000000000000000000001';
    const address = `000000000000000000000000${bet.address.substring(2)}`
    const betAmount = leftPadValue(hexlify(bet.amount).substring(2));
    const betData = leftPadValue(hexlify(bet.data).substring(2));

    return `${uint256Type}${roundId}${arrayType}${arrayLength}${uint256Type}${address}${betAmount}${storageType}${uint256Type}${betData}`;
  }

  // public async handlePlay(bet: Bet) {

  //   //TODO: move to game.ts
  //   const state = store.getState();
  //   bet.roundId = state.game.round?.id as string;
  //   bet.address = state.user.address as string;

  //   if (!bet.roundId) {
  //     store.dispatch(setUserError('Invalid round ID'));
  //     throw new Error('Error placing bet. Round id not found');
  //   }

  //   const abi = new Interface([
  //     {
  //       anonymous: false,
  //       inputs: [
  //         {
  //           indexed: true,
  //           name: '_roundID',
  //           type: 'bytes32',
  //         },
  //         {
  //           components: [
  //             {
  //               name: 'playerAddress',
  //               type: 'address',
  //             },
  //             {
  //               name: 'betAmount',
  //               type: 'uint256',
  //             },
  //             {
  //               name: 'betData',
  //               type: 'bytes',
  //             },
  //           ],
  //           indexed: false,
  //           name: '_bet',
  //           type: 'tuple[]',
  //         },
  //       ],
  //       name: 'bet',
  //       stateMutability: 'nonpayable',
  //       type: 'function',
  //     },
  //   ]);

  //   const calldata = abi.encodeFunctionData('bet', [
  //     bet.roundId,
  //     [
  //       {
  //         playerAddress: bet.address,
  //         betAmount: hexlify(bet.amount),
  //         betData: hexlify(bet.data),
  //       },
  //     ],
  //   ]);

  //   const contract = await ethers.getContract<LabRatsToken>(
  //     LabRatsTokenABI,
  //     this.TOKEN_ADDRESS
  //   );
  //   const transactionResponse = await contract.transferAndCall(
  //     this.GAME_MANAGER_ADDRESS,
  //     bet.amount,
  //     calldata
  //   );
  //   const receipt = await transactionResponse.wait(1);
  //   console.log('handlePlay receipt: ', receipt);
  //   // TODO: dispatch confirmation to store
  // }
  
  public handleBalanceFreeze = (frozen: boolean) => {
    if(frozen) {
      store.dispatch(freezeDisplayBalance());
    } else {
      store.dispatch(unFreezeDisplayBalance());
    }
  }

  public handlePlay = async (stake: number, action: number) => {

    const state = store.getState();
    const bet: Bet = {
      roundId: state.game.round?.id as string,
      address: state.user.address as string,
      amount: stake,
      data: hexlify(action),
      confirmed: false,
    } 
    
    if (!bet.roundId) {
      store.dispatch(setGameError({code: ErrorCode.GENERAL_BET_ERROR, msg: 'Invalid round ID'}));
      throw new Error('Error placing bet. Round id not found');
    }

    const encoded = ethers.encode(
      [
        {
            name: 'TokenTransferData',
            indexed: false,
            type: 'tuple',
            components: [
              {
                name: 'roundID',
                indexed: false,
                type: 'bytes32',
              },
              {
                name: 'bets',
                indexed: false,
                type: 'tuple[]',
                components: [
                  {
                    name: 'playerAddress',
                    indexed: false,
                    type: 'address',
                  },
                  {
                    name: 'betAmount',
                    indexed: false,
                    type: 'uint256',
                  },
                  {
                    name: 'betData',
                    indexed: false,
                    type: 'bytes',
                  }
                ]
              }
            ]
        }
      ],
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
        }

      ]
    );

    const contract = await ethers.getContract<LabRatsToken>(
      LabRatsTokenABI,
      this.TOKEN_ADDRESS
    );

    // console.log('++ sending transferAndCall');
    // console.log('++ roundId:', bet.roundId);
    // console.log('++ address:', this.TOKEN_ADDRESS);
    // console.log('++ amount:', bet.amount.toString());
    // console.log('++ unencoded data:', hexlify(bet.data));
    // console.log('++ encoded data:', encoded);
    try{
      const transactionResponse = await contract.transferAndCall(
        this.GAME_MANAGER_ADDRESS,
        bet.amount,
        encoded
      );
      
      messageService.broadcastBet(bet);
  
      const receipt = await transactionResponse.wait(1);
      console.log('handlePlay receipt: ', receipt);
    } catch(error) {
      console.error(error);
      store.dispatch(setGameError({code: ErrorCode.GENERAL_BET_ERROR, msg: 'Error placing bet'}))
    }

    // TODO: dispatch confirmation to store
  }

  // public async testForRoundStart(blockHeader: BlockHeader) {
  //   const eventName = 'StartGameRound';
  //   // TODO: isInBloom ... [contract, eventName, roundId]
  //   const contract = await ethers.getContract<MultiplayerGamesManager>(MultiplayerGamesManagerABI, this.GAME_MANAGER_ADDRESS);
  //   const events: Event[] = await (contract as any).queryFilter(
  //     eventName,
  //     blockHeader.blockHash
  //   );

  //   events.forEach((event: Event) => {
  //     // const result: RoundResult = { test: event };
  //     //@ts-ignore
  //     store.dispatch(setRoundId(event._roundID));
  //   });
  // }

  public async testForBetEvents(blockHeader: BlockHeader) {
    const eventName = 'Bet';
    const state = store.getState();
    const contractInBloom = isContractAddressInBloom(blockHeader.bloomFilter, this.GAME_MANAGER_ADDRESS);
    const roundInBloom = !!state.game.round ? isInBloom(blockHeader.bloomFilter, state.game.round.id) : false;
    
    if(!contractInBloom || !roundInBloom) {
      return;
    }

    const contract = await ethers.getContract<MultiplayerGamesManager>(MultiplayerGamesManagerABI, this.GAME_MANAGER_ADDRESS);
    const events: Event[] = await (contract as any).queryFilter(
      eventName,
      blockHeader.blockHash
    ).catch((error: any) => {
      console.error(error);
      store.dispatch(setNetworkError({code: ErrorCode.JSON_RPC_READ_ERROR, msg: `Error reading events for block ${blockHeader.blockNumber}`}));
      return [];
    });;

    events.forEach((event: Event) => {
      const bet: Bet|null = !!event.args ? {
        roundId: event.args[BetEvent.ROUND_ID], 
        address: event.args[BetEvent.DATA]['playerAddress'],
        amount: event.args[BetEvent.DATA]['betAmount'],
        data: event.args[BetEvent.DATA]['betData'],
        confirmed: true,
      } : null;

      if(bet){
        store.dispatch(addBet(bet));
      }
    });
  }

  public async testForRoundResult(blockHeader: BlockHeader) {
    const eventName = 'EndGameRound';
    const state = store.getState();

    const contractInBloom = isContractAddressInBloom(blockHeader.bloomFilter, this.GAME_MANAGER_ADDRESS);
    const roundInBloom = !!state.game.round ? isInBloom(blockHeader.bloomFilter, state.game.round.id) : false;
    
    if(!contractInBloom || !roundInBloom) {
      return;
    }

    const contract = await ethers.getContract<MultiplayerGamesManager>(MultiplayerGamesManagerABI, this.GAME_MANAGER_ADDRESS);
    const events: Event[] = await (contract as any).queryFilter(
      eventName,
      blockHeader.blockHash
    ).catch((error: any) => {
      console.error(error);
      store.dispatch(setNetworkError({code: ErrorCode.JSON_RPC_READ_ERROR, msg: `Error reading events for block ${blockHeader.blockNumber}`}));
      return [];
    });

    events.forEach(async(event) => {
      const persistentData: GetPersistentGameDataByIDResponse|null = event.args ? 
      await contract.getPersistentGameDataByID(event.args[EndGameRound.PERSISTENT_GAME_DATA_ID]).catch(error => {
        console.error(error);
        store.dispatch(setGameError({code: ErrorCode.JSON_RPC_READ_ERROR, msg: 'Error reading persistant game data'}));
        return null;
      }) 
      : null;

      const result: RoundResult|null = !!event.args ? {
        id: event.args[EndGameRound.ROUND_ID],
        playerAddresses: event.args[EndGameRound.PLAYER_ADDRESSES],
        winAmounts: event.args[EndGameRound.WIN_AMOUNTS],
        result: event.args[EndGameRound.GAME_RESULT],
        history: event.args[EndGameRound.HISTORY],
        potWinLoss: event.args[EndGameRound.POT_WIN_LOSS],
        entropyReveal: event.args[EndGameRound.ENTROPY_REVEAL],
        persistentGameData: !!persistentData ? {
          id: event.args[EndGameRound.PERSISTENT_GAME_DATA_ID],
          data: persistentData[PersistentDataEvent.GAME_DATA],
          pot: persistentData[PersistentDataEvent.POT_VALUE].toNumber()
        } : null,
      } : null;

      if(!!result) {
        store.dispatch(setResult(result));
        store.dispatch(clearBets());
      }
    });
  }

  public async readBlockHeader(blockHeader: BlockHeader) {
    this.testForBetEvents(blockHeader);
    this.testForRoundResult(blockHeader);
  }

}

export const gameService = new GameService();
