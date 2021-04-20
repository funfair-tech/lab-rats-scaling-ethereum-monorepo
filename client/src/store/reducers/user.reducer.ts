import { SET_ADDRESS, SET_AUTHENTICATED, SET_TOKEN_BALANCE, UserActionTypes } from '../types/user.types';

export interface User {
  authenticated: boolean;
  tokenBalance: number|undefined;
  address: string;
}

const initialState: User = {
  authenticated: false,
  tokenBalance: undefined,
  address: '',
};

const userReducer = (state = initialState, action: UserActionTypes): User => {
  switch (action.type) {
    case SET_AUTHENTICATED:
      return {
        ...state,
        authenticated: action.payload,
      };
    case SET_TOKEN_BALANCE:
      return {
        ...state,
        tokenBalance: action.payload,
      };
    case SET_ADDRESS:
      return {
        ...state,
        address: action.payload,
      };
    default: {
      return state;
    }
  }
}

export default userReducer;
