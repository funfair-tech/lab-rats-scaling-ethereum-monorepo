import { hexlify, Interface } from 'ethers/lib/utils';
import ConnectionTest from '../contracts/connectionTest.json';
import { IConnectionTest } from '../contracts/connetionTest';
import { LabRatsToken } from '../contracts/LabRatsToken';
import LabRatsTokenABI from '../contracts/labRatsToken.json';
import { Bet } from '../model/bet';
import { BlockHeader } from '../model/blockHeader';
import { RoundResult } from '../model/roundResult';
import { setResult } from '../store/actions/game.actions';
import { setUserError } from '../store/actions/user.actions';
import store from '../store/store';
import { ethers } from './ether.service';

class GameService {
  private testAddress = '0xf4Ca8a4a571Fbac6DB7e7824A1F97CC68058FB7d';
  private GAME_ADDRESS = '0xFc35436FecCeC70Ad223dC88B2eba647846F3170';
  private TOKEN_ADDRESS = '0x11160251d4283A48B7A8808aa0ED8EA5349B56e2';

  public async callTest() {
    const contract = await ethers.getContract<IConnectionTest>(
      ConnectionTest,
      this.testAddress
    );
    const message: string = await contract.getMessage();
    console.log('test: getMessage ', message);
  }

  public async sendTest() {
    const contract = await ethers.getContract<IConnectionTest>(
      ConnectionTest,
      this.testAddress
    );
    const transactionResponse = await contract.setMessage('bar');
    const receipt = await transactionResponse.wait(1);
    console.log('test transaction confirmed: ', receipt.status);
  }

  public async handlePlay(bet: Bet) {
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
      this.testAddress
    );
    const transactionResponse = await contract.transferAndCall(
      this.GAME_ADDRESS,
      bet.amount.toString(),
      calldata
    );
    const receipt = await transactionResponse.wait(1);
    console.log('handlePlay receipt: ', receipt);
    // TODO: dispatch confirmation to store
  }
  
  
  public async handlePlayWithAbiCoder(bet: Bet) {

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
      this.testAddress
    );
    const transactionResponse = await contract.transferAndCall(
      this.GAME_ADDRESS,
      bet.amount.toString(),
      encoded
    );
    const receipt = await transactionResponse.wait(1);
    console.log('handlePlay receipt: ', receipt);
    // TODO: dispatch confirmation to store
  }

  public async testForRoundResult(blockHeader: BlockHeader) {
    const eventName = 'MessageSet';
    // TODO: isInBloom ... [contract, eventName, roundId]
    const abi = ConnectionTest;
    const address = this.testAddress;
    const contract = await ethers.getContract<IConnectionTest>(abi, address);
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
