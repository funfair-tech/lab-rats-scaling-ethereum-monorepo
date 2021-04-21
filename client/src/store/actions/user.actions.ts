import { CLEAR_USER_STATE, SET_ADDRESS, SET_AUTHENTICATED, SET_ETH_BALANCE, SET_LOADING, SET_TOKEN_BALANCE, UserActionTypes } from '../types/user.types';

export const setAuthenticated = (authenticated: boolean): UserActionTypes => {
  return {
    type: SET_AUTHENTICATED,
    payload: authenticated,
  };
};

export const setEthBalance = (ethBalance: number): UserActionTypes => {
  return {
    type: SET_ETH_BALANCE,
    payload: ethBalance,
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

export const setLoading = (isLoading: boolean): UserActionTypes => {
  return {
    type: SET_LOADING,
    payload: isLoading,
  };
};

export const clearUserState = (): UserActionTypes => {
  return {
    type: CLEAR_USER_STATE,
  };
};
