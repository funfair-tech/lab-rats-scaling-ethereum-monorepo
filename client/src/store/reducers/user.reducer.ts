import {
  CLEAR_USER_STATE,
  SET_ADDRESS,
  SET_AUTHENTICATED,
  SET_ETH_BALANCE,
  SET_LOADING,
  SET_TOKEN_BALANCE,
  SET_USER_ERROR,
  UserActionTypes,
} from '../types/user.types';

export interface User {
  error: string|null;
  authenticated: boolean;
  ethBalance: number;
  tokenBalance: number;
  address: string;
  loading: boolean;
}

const initialState: User = {
  error: null,
  authenticated: false,
  ethBalance: 0,
  tokenBalance: 0,
  address: '',
  loading: true,
};

const userReducer = (state = { ...initialState }, action: UserActionTypes): User => {
  switch (action.type) {
    case SET_USER_ERROR:
      return {
        ...state,
        error: action.payload,
      };
    case CLEAR_USER_STATE:
      return {
        ...initialState,
        loading: false,
      };
    case SET_AUTHENTICATED:
      return {
        ...state,
        authenticated: action.payload,
      };
    case SET_ETH_BALANCE:
      return {
        ...state,
        ethBalance: action.payload,
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
    case SET_LOADING:
      return {
        ...state,
        loading: action.payload,
      };
    default: {
      return state;
    }
  }
};

export default userReducer;
