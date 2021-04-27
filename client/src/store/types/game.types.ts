import { RoundResult } from '../../model/roundResult';

export const CLEAR_GAME_STATE = 'CLEAR_GAME_STATE';
export const SET_RESULT = 'SET_RESULT';
export const SET_PLAYERS_ONLINE = 'SET_PLAYERS_ONLINE';

export interface ClearGameState {
  type: typeof CLEAR_GAME_STATE;
}

export interface SetResult {
  type: typeof SET_RESULT;
  payload: RoundResult;
}

export interface SetPlayersOnline {
  type: typeof SET_PLAYERS_ONLINE;
  payload: number;
}

export type GameActionTypes = ClearGameState | SetResult | SetPlayersOnline;
