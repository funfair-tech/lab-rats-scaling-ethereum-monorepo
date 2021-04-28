import { Bet } from '../../model/bet';
import { Round } from '../../model/round';
import { RoundResult } from '../../model/roundResult';
import { ADD_BET, CLEAR_BETS, CLEAR_GAME_STATE, GameActionTypes, SET_CAN_PLAY, SET_PLAYERS_ONLINE, SET_RESULT, SET_ROUND } from '../types/game.types';

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

export const setRound = (round: Round|null): GameActionTypes => {
  return {
    type: SET_ROUND,
    payload: round,
  };
};

export const setCanPlay = (canPlay: boolean): GameActionTypes => {
  return {
    type: SET_CAN_PLAY,
    payload: canPlay,
  };
};

export const addBet = (bet: Bet): GameActionTypes => {
  return {
    type: ADD_BET,
    payload: bet,
  };
};

export const clearBets = (): GameActionTypes => {
  return {
    type: CLEAR_BETS,
  };
};
