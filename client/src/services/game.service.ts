import { hexlify, Interface } from 'ethers/lib/utils';
import { LabRatsToken } from '../contracts/LabRatsToken';
import LabRatsTokenABI from '../contracts/labRatsToken.json';
import { MultiplayerGamesManager } from '../contracts/MultiplayerGamesManager';
import MultiplayerGamesManagerABI from '../contracts/multiplayerGamesManager.json';
import { Bet } from '../model/bet';
import { BlockHeader } from '../model/blockHeader';
import { RoundResult } from '../model/roundResult';
import { setResult, setRoundId } from '../store/actions/game.actions';
import { setUserError } from '../store/actions/user.actions';
import store from '../store/store';
import { ethers } from './ether.service';

class GameService {
  // private GAME_ADDRESS = '0xBB5Da74b1bAEFD5a928aB63387e698803A8Cc0B9';
  private GAME_MANAGER_ADDRESS = '0xe3f2Fa6a3F16837d012e1493F50BD29db0BdADe4';
  private TOKEN_ADDRESS = '0x11160251d4283A48B7A8808aa0ED8EA5349B56e2';

  public async handlePlay(bet: Bet) {

    //TODO: move to game.ts
    const state = store.getState();
    bet.roundId = state.game.roundId as string;
    bet.address = state.user.address as string;

    if (!bet.roundId) {
      store.dispatch(setUserError('Invalid round ID'));
      throw new Error('Error placing bet. Round id not found');
    }

    const abi = new Interface([
      {
        anonymous: false,
        inputs: [
          {
            indexed: true,
            name: '_roundID',
            type: 'bytes32',
          },
          {
            components: [
              {
                name: 'playerAddress',
                type: 'address',
              },
              {
                name: 'betAmount',
                type: 'uint256',
              },
              {
                name: 'betData',
                type: 'bytes',
              },
            ],
            indexed: false,
            name: '_bet',
            type: 'tuple[]',
          },
        ],
        name: 'bet',
        stateMutability: 'nonpayable',
        type: 'function',
      },
    ]);

    const calldata = abi.encodeFunctionData('bet', [
      bet.roundId,
      [
        {
          playerAddress: bet.address,
          betAmount: hexlify(bet.amount),
          betData: hexlify(bet.data),
        },
      ],
    ]);

    const contract = await ethers.getContract<LabRatsToken>(
      LabRatsTokenABI,
      this.TOKEN_ADDRESS
    );
    const transactionResponse = await contract.transferAndCall(
      this.TOKEN_ADDRESS,
      bet.amount.toString(),
      calldata
    );
    const receipt = await transactionResponse.wait(1);
    console.log('handlePlay receipt: ', receipt);
    // TODO: dispatch confirmation to store
  }
  
  


  public async handlePlayWithAbiCoder(bet: Bet) {

    //TODO: move to game.ts
    const state = store.getState();
    bet.roundId = state.game.roundId as string;
    bet.address = state.user.address as string;
    
    if (!bet.roundId) {
      store.dispatch(setUserError('Invalid round ID'));
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
          playerAddress: bet.address,
          betAmount: hexlify(bet.amount),
          betData: hexlify(bet.data),
        },
      ]
    );

    const contract = await ethers.getContract<LabRatsToken>(
      LabRatsTokenABI,
      this.TOKEN_ADDRESS
    );
    const transactionResponse = await contract.transferAndCall(
      this.TOKEN_ADDRESS,
      bet.amount.toString(),
      encoded
    );
    const receipt = await transactionResponse.wait(1);
    console.log('handlePlay receipt: ', receipt);
    // TODO: dispatch confirmation to store
  }

  public async testForRoundStart(blockHeader: BlockHeader) {
    const eventName = 'StartGameRound';
    // TODO: isInBloom ... [contract, eventName, roundId]
    const contract = await ethers.getContract<MultiplayerGamesManager>(MultiplayerGamesManagerABI, this.GAME_MANAGER_ADDRESS);
    const events: Event[] = await (contract as any).queryFilter(
      eventName,
      blockHeader.blockHash
    );

    events.forEach((event: Event) => {
      // const result: RoundResult = { test: event };
      //@ts-ignore
      store.dispatch(setRoundId(event._roundID));
    });
  }

  public async testForRoundResult(blockHeader: BlockHeader) {
    const eventName = 'EndGameRound';
    // TODO: isInBloom ... [contract, eventName, roundId]
    const contract = await ethers.getContract<MultiplayerGamesManager>(MultiplayerGamesManagerABI, this.GAME_MANAGER_ADDRESS);
    const events: Event[] = await (contract as any).queryFilter(
      eventName,
      blockHeader.blockHash
    );

    events.forEach((event) => {
      const result: RoundResult = { test: event };
      store.dispatch(setResult(result));
    });
  }
}

export const gameService = new GameService();
