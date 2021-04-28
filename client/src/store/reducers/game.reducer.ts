import { Bet } from '../../model/bet';
import { Round } from '../../model/round';
import { RoundResult } from '../../model/roundResult';
import { ADD_BET, CLEAR_BETS, CLEAR_GAME_STATE, GameActionTypes, SET_CAN_PLAY, SET_PLAYERS_ONLINE, SET_RESULT, SET_ROUND } from '../types/game.types';


export interface Game {
  round: Round|null;
  bets: Bet[];
  result: RoundResult|null;
  playersOnline: number|null;
  canPlay: boolean;
}

const initialState: Game = {
  round: null,
  bets:[],
  result: null,
  playersOnline: null,
  canPlay: false,
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
    case SET_PLAYERS_ONLINE:
      return {
        ...state,
        playersOnline: action.payload,
      };
    case SET_ROUND:
      return {
        ...state,
        round: action.payload,
      };
    case SET_CAN_PLAY:
      return {
        ...state,
        canPlay: action.payload,
      };
    case ADD_BET:
      return {
        ...state,
        bets: [...state.bets, action.payload],
      };
    case CLEAR_BETS:
      return {
        ...state,
        bets: [],
      };
    default: {
      return state;
    }
  }
};

export default gameReducer;
