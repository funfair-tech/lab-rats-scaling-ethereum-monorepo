export const SET_AUTHENTICATED = 'SET_AUTHENTICATED';
export const SET_ETH_BALANCE = 'SET_ETH_BALANCE';
export const SET_TOKEN_BALANCE = 'SET_TOKEN_BALANCE';
export const SET_ADDRESS = 'SET_ADDRESS';
export const SET_LOADING = 'SET_LOADING';
export const CLEAR_USER_STATE = 'CLEAR_USER_STATE';

export interface SetAuthenticatedAction {
  type: typeof SET_AUTHENTICATED;
  payload: boolean;
}

export interface SetEthBalanceAction {
  type: typeof SET_ETH_BALANCE;
  payload: number;
}

export interface SetTokenBalanceAction {
  type: typeof SET_TOKEN_BALANCE;
  payload: number;
}

export interface SetAddressAction {
  type: typeof SET_ADDRESS;
  payload: string;
}

export interface SetLoading {
  type: typeof SET_LOADING;
  payload: boolean;
}

export interface ClearUserState {
  type: typeof CLEAR_USER_STATE;
}

export type UserActionTypes = SetAuthenticatedAction | SetEthBalanceAction | SetTokenBalanceAction | SetAddressAction | SetLoading | ClearUserState;
