import { Bet } from '../../model/bet';
import { Round } from '../../model/round';
import { RoundResult } from '../../model/roundResult';
import {
  ADD_BET,
  CLEAR_BETS,
  CLEAR_GAME_STATE,
  GameActionTypes,
  SET_CAN_PLAY,
  SET_GAME_ERROR,
  SET_HISTORY,
  SET_PLAYERS_ONLINE,
  SET_RESULT,
  SET_ROUND,
} from '../types/game.types';

export interface Game {
  error: string | null;
  round: Round | null;
  bets: Bet[];
  result: RoundResult | null;
  playersOnline: number | null;
  canPlay: boolean;
  history: string|null;
}

const initialState: Game = {
  error: null,
  round: null,
  bets: [],
  result: null,
  playersOnline: null,
  canPlay: false,
  history: null
};

const gameReducer = ( state = { ...initialState }, action: GameActionTypes): Game => {
  switch (action.type) {
  
    case CLEAR_GAME_STATE:
      return initialState;
    case SET_GAME_ERROR:
      return {
        ...state,
        error: action.payload,
      };
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
        canPlay: true,
        round: action.payload,
      };
    case SET_CAN_PLAY:
      return {
        ...state,
        canPlay: action.payload,
      };
    case ADD_BET:
      const found = state.bets.find(
        (bet) => bet.address === action.payload.address
      );
      return found
        ? {
            ...state,
            bets: state.bets.map((bet) =>
              bet.address === action.payload.address
                ? { ...bet, confirmed: action.payload.confirmed }
                : bet
            ),
          }
        : {
            ...state,
            bets: [...state.bets, action.payload],
          };
    case CLEAR_BETS:
      return {
        ...state,
        bets: [],
      };
    case SET_HISTORY:
      return {
        ...state,
        history: action.payload,
      };
    default: {
      return state;
    }
  }
};

export default gameReducer;
