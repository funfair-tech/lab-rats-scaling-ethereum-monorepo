import { ethers } from './ether.service';
import { BlockHeader } from '../model/blockHeader';
import { RoundResult } from '../model/roundResult';
import { setResult } from '../store/actions/game.actions';
import store from '../store/store';
import { RoundAction } from '../model/roundAction';
import ConnectionTest from '../contracts/connectionTest.json';
import { IConnectionTest } from '../contracts/connetionTest';
import { TransactionResponse } from '@ethersproject/abstract-provider';

class GameService {
  
  private testAddress = '0xf4Ca8a4a571Fbac6DB7e7824A1F97CC68058FB7d';

  public async callTest() {
    const contract = await ethers.getContract<IConnectionTest>(ConnectionTest, this.testAddress);
    const message: string = await contract.getMessage();
    console.log('test: getMessage ', message);
  }

  public async sendTest() {
    const contract = await ethers.getContract<IConnectionTest>(ConnectionTest, this.testAddress);
    const transactionResponse = await contract.setMessage('bar');
    // console.log('transcation response: ', transactionResponse);
    const receipt = await transactionResponse.wait(1);
    console.log('test transaction confirmed: ', receipt.status);

  }

  public async handlePlay(action: RoundAction) {
    // const abi = ConnectionTest;
    // const contract = await ethers.getContract<IConnectionTest>(ConnectionTest, this.testAddress);
    // const transactionResponse = await contract.setMessage('bar');
    // const receipt = await transactionResponse.wait(1);
    // TODO: dispatch confirmation to store
  }

  public async testForRoundResult(blockHeader: BlockHeader) {
    const eventName = 'MessageSet';
    // TODO: isInBloom ... [contract, eventName, roundId]
    const abi = ConnectionTest;
    const address = this.testAddress;
    const contract = await ethers.getContract<IConnectionTest>(abi, address);
    const events: Event[] = await (contract as any).queryFilter( eventName , blockHeader.blockHash );
    
    events.forEach(event => {
      const result: RoundResult = {test: event};
      store.dispatch(setResult(result));
    })
  }
}

export const gameService = new GameService();