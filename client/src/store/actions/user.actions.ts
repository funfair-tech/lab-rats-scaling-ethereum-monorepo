import { LRError } from '../../model/errorCodes';
import {
  CLEAR_USER_STATE,
  FREEZE_DISPLAY_BALANCE,
  SET_ADDRESS,
  SET_AUTHENTICATED,
  SET_ETH_BALANCE,
  SET_LOADING,
  SET_TOKEN_BALANCE,
  SET_USER_ERROR,
  UNFREEZE_DISPLAY_BALANCE,
  UserActionTypes,
} from '../types/user.types';

export const setUserError = (error: LRError | null): UserActionTypes => {
  return {
    type: SET_USER_ERROR,
    payload: error,
  };
};

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

export const freezeDisplayBalance = (): UserActionTypes => {
  return {
    type: FREEZE_DISPLAY_BALANCE,
  };
};

export const unFreezeDisplayBalance = (): UserActionTypes => {
  return {
    type: UNFREEZE_DISPLAY_BALANCE,
  };
};
