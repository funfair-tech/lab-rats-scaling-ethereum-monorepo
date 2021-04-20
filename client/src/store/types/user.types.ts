export const SET_AUTHENTICATED = 'SET_AUTHENTICATED';
export const SET_TOKEN_BALANCE = 'SET_TOKEN_BALANCE';
export const SET_ADDRESS = 'SET_ADDRESS';

export interface SetAuthenticatedAction {
  type: typeof SET_AUTHENTICATED;
  payload: boolean;
}

export interface SetTokenBalanceAction {
  type: typeof SET_TOKEN_BALANCE;
  payload: number;
}

export interface SetAddressAction {
  type: typeof SET_ADDRESS;
  payload: string;
}

export type UserActionTypes = SetAuthenticatedAction | SetTokenBalanceAction | SetAddressAction;
