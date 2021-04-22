import { RoundResult } from '../../model/roundResult';
import { CLEAR_GAME_STATE, GameActionTypes, SET_RESULT } from '../types/game.types';


export interface Game {
  result: RoundResult|null;
}

const initialState: Game = {
  result: null,
};

const gameReducer = (state = { ...initialState }, action: GameActionTypes): Game => {
  switch (action.type) {
    case CLEAR_GAME_STATE:
      return initialState;
    case SET_RESULT:
      return {
        ...state,
        result: action.payload,
      };
    default: {
      return state;
    }
  }
};

export default gameReducer;
