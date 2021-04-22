import { BlockHeader } from '../model/blockHeader';
import { RoundResult } from '../model/roundResult';
import { setResult } from '../store/actions/game.actions';
import store from '../store/store';

class GameService {
  
  public handlePlay(event: any) {
    console.log('handlePlay: action : ', event);
    // TODO: send play tx 
  }

  public testForRoundResult(blockHeader: BlockHeader) {
    // TODO: filter out blocks without game events
    //       read events


    // update the store
    const result: RoundResult = {test:blockHeader.blockNumber};
    store.dispatch(setResult(result))
  }

}

export const gameService = new GameService();
