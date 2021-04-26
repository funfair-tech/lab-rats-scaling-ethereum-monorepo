import { ethers } from './ether.service';
import { BlockHeader } from '../model/blockHeader';
import { RoundResult } from '../model/roundResult';
import { setResult } from '../store/actions/game.actions';
import store from '../store/store';
import { RoundAction } from '../model/roundAction';
import ConnectionTest from '../contracts/connectionTest.json';
import { IConnectionTest } from '../contracts/connetionTest';

class GameService {
  
  private testAddress = '0xf4Ca8a4a571Fbac6DB7e7824A1F97CC68058FB7d';
  public async callTest() {
    const contract = await ethers.getContract<IConnectionTest>(ConnectionTest, this.testAddress);
    const message: string = await contract.getMessage();
    console.log('test: getMessage ', message);
  }

  public async sendTest() {
    const contract = await ethers.getContract<IConnectionTest>(ConnectionTest, this.testAddress);
    contract.setMessage(ethers.formatString('bar'));
    // console.log('test: getMessage ', message);
  }

  public handlePlay(action: RoundAction) {
    // console.log('handlePlay: action : ', action);
    // // TODO: send play tx
    // const abi = '';
    // const address = '';

    // ethers.getContract(abi, address).play();
  }

  public testForRoundResult(blockHeader: BlockHeader) {
    // TODO: filter out blocks without game events
    //       read events
    // const abi = '';
    // const address = '';
    // const eventName = '';
    // const fromBlock = blockHeader.blockNumber;
    // const events = ethers.getContract(abi, address).queryFilter(eventName, fromBlock);

    // // update the store
    // const result: RoundResult = {test:blockHeader.blockNumber};
    // store.dispatch(setResult(result))
  }

}

export const gameService = new GameService();