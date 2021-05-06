import { hexlify } from 'ethers/lib/utils';
import { LabRatsToken } from '../contracts/LabRatsToken';
import LabRatsTokenABI from '../contracts/labRatsToken.json';
import { GetPersistentGameDataByIDResponse, MultiplayerGamesManager } from '../contracts/MultiplayerGamesManager';
import MultiplayerGamesManagerABI from '../contracts/multiplayerGamesManager.json';
import { Bet } from '../model/bet';
import { BlockHeader } from '../model/blockHeader';
import { RoundResult } from '../model/roundResult';
import { addBet, clearBets, setCanPlay, setGameError, setResult, setRound } from '../store/actions/game.actions';
import store from '../store/store';
import { ethers } from './ether.service';
import { messageService } from './message.service';
import { isContractAddressInBloom, isInBloom } from 'ethereum-bloom-filters';
import { Event } from '@ethersproject/contracts';
import { EndGameRound, IEndGameRound } from '../events/endGameRound';
import { BetEvent, IBet } from '../events/betEvent';
import { PersistentDataEvent } from '../events/persistentDataEvent';
import { ErrorCode } from '../model/errorCodes';
import { setNetworkError } from '../store/actions/network.actions';
import { freezeDisplayBalance, unFreezeDisplayBalance } from '../store/actions/user.actions';
import { Round } from '../model/round';
import { INoMoreBets, NoMoreBets } from '../events/noMoreBets';
import { IStartGameRound } from '../events/startGameRound';
import { eventNames } from 'node:process';
class GameService {
  // private GAME_MANAGER_ADDRESS = '0x14FE1360ba12F1b84f3429f85138A0B8896A01Ca';
  private GAME_MANAGER_ADDRESS = '0x832B7d868C45a53e9690ffc12527391098bBd0dD';
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
      // Confirmation comes from the bet event. See testForBetEvents below.
  }

/**
 * event NoMoreBets(bytes32 indexed _roundID, bytes32 indexed _persistentGameDataID);    

 * @param blockHeader 
 */

  public async testForNoMoreBets(blockHeader: BlockHeader) {
    const eventName = 'NoMoreBets';

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
      if(!!event.args && event.args[NoMoreBets.ROUND_ID] === state.game.round?.id) {
        store.dispatch(setCanPlay(false));
      }
    });
  }

  public async testForRoundStart(blockHeader: BlockHeader) {
    const eventName = 'StartGameRound';
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
      console.log('++ start round event ', event);

      const round : Round|null = !!event.args ?{
        id: event.args[BetEvent.ROUND_ID],
        block: blockHeader.blockNumber,
        time: 0,
        timeToNextRound: 0,
      } : null

      if(round) {
        store.dispatch(setRound(round));
      }
    });
  }

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
    // this.testForBetEvents(blockHeader);
    // this.testForRoundResult(blockHeader);
    // this.testForRoundStart(blockHeader);
    // this.testForNoMoreBets(blockHeader);
  }


  ////////////////////////////////////////////////////////////////////////////////////////
  public async subscribeToContractEvents() {
    const contract = await ethers.getContract<MultiplayerGamesManager>(MultiplayerGamesManagerABI, this.GAME_MANAGER_ADDRESS);

    (contract as any).on( 'StartGameRound' , (from:string, to: string, event: IStartGameRound) => {
      console.log('## start round event from', from);
      console.log('## start round event to', to);
      console.log('## start round event ', event);
      
      const round : Round = {
        id: event.roundID,
        //TODO: remove redundant fields
        block: 0,
        time: 0,
        timeToNextRound: 0,
      }

      store.dispatch(setRound(round));
    });

    (contract as any).on( 'Bet' , (from:string, to: string, event: IBet) => {
      console.log('## bet event from', from);
      console.log('## bet event to', to);
      console.log('## bet event ', event);

      const bet: Bet = {
        roundId: event.roundId, 
        address: event.data.playerAddress,
        amount: event.data.betAmount,
        data: event.data.betData,
        confirmed: true,
      };

        store.dispatch(addBet(bet));
    });

    (contract as any).on( 'NoMoreBets' , (from:string, to: string, event: INoMoreBets) => {
      console.log('## no more bets event from', from);
      console.log('## no more bets event to', to);
      console.log('## no more bets event ', event);
      const state = store.getState();
      if( event.roundId === state.game.round?.id) {
        store.dispatch(setCanPlay(false));
      }
    });

    (contract as any).on( 'EndGameRound' , async (from:string, to: string, event: IEndGameRound) => {
      console.log('## end game round event from', from);
      console.log('## end game round event to', to);
      console.log('## end game round event ', event);

      const persistentData: GetPersistentGameDataByIDResponse|null = await contract.getPersistentGameDataByID(event.persistentGameDataID).catch(error => {
        console.error(error);
        store.dispatch(setGameError({code: ErrorCode.JSON_RPC_READ_ERROR, msg: 'Error reading persistant game data'}));
        return null;
      }) 

      const result: RoundResult = {
        id: event.roundId,
        playerAddresses: event.playerAddresses,
        winAmounts: event.winAmounts,
        result: event.result,
        history: event.history,
        potWinLoss: event.persistentGameDataPotWinLoss,
        entropyReveal: event.entropyReveal,
        persistentGameData: !!persistentData ? {
          id: event.persistentGameDataID,
          data: persistentData[PersistentDataEvent.GAME_DATA],
          pot: persistentData[PersistentDataEvent.POT_VALUE].toNumber()
        } : null,
      };

      if(!!result) {
        store.dispatch(setResult(result));
        store.dispatch(clearBets());
      }
    });
  }
  ////////////////////////////////////////////////////////////////////////////////////////

}

export const gameService = new GameService();
