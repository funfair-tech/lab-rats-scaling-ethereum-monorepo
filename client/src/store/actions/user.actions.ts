import { SET_ADDRESS, SET_AUTHENTICATED, SET_TOKEN_BALANCE, UserActionTypes } from '../types/user.types';

export const setAuthenticated = (authenticated: boolean): UserActionTypes => {
  return {
    type: SET_AUTHENTICATED,
    payload: authenticated,
  };
};

export const setTokenBalance = (tokenBalance: number): UserActionTypes => {
  return {
    type: SET_TOKEN_BALANCE,
    payload: tokenBalance,
  };
};

export const setAddress = (address: string): UserActionTypes => {
  return {
    type: SET_ADDRESS,
    payload: address,
  };
};
