import { Bet } from '../../model/bet';
import { RoundResult } from '../../model/roundResult';

export const CLEAR_GAME_STATE = 'CLEAR_GAME_STATE';
export const SET_RESULT = 'SET_RESULT';
export const SET_PLAYERS_ONLINE = 'SET_PLAYERS_ONLINE';
export const SET_ROUND_ID = 'SET_ROUND_ID';
export const SET_CAN_PLAY = 'SET_CAN_PLAY';
export const ADD_BET = 'ADD_BET';
export const CLEAR_BETS = 'CLEAR_BETS';

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

export interface SetRoundId {
  type: typeof SET_ROUND_ID;
  payload: string | null;
}

export interface SetCanPlay {
  type: typeof SET_CAN_PLAY;
  payload: boolean;
}

export interface AddBet {
  type: typeof ADD_BET;
  payload: Bet;
}

export interface ClearBets {
  type: typeof CLEAR_BETS;
}

export type GameActionTypes =
  | ClearGameState
  | SetResult
  | SetPlayersOnline
  | SetRoundId
  | SetCanPlay
  | AddBet
  | ClearBets;
