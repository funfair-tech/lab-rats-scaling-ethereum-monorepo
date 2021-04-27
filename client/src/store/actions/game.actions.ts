import { RoundResult } from '../../model/roundResult';
import { CLEAR_GAME_STATE, GameActionTypes, SET_PLAYERS_ONLINE, SET_RESULT } from '../types/game.types';

export const setResult = (result: RoundResult): GameActionTypes => {
  return {
    type: SET_RESULT,
    payload: result,
  };
};

export const clearGameState = (): GameActionTypes => {
  return {
    type: CLEAR_GAME_STATE,
  };
};

export const setPlayersOnline = (playersOnline: number): GameActionTypes => {
  return {
    type: SET_PLAYERS_ONLINE,
    payload: playersOnline,
  };
};
