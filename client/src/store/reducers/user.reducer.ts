import {
  CLEAR_USER_STATE,
  SET_ADDRESS,
  SET_AUTHENTICATED,
  SET_ETH_BALANCE,
  SET_LOADING,
  SET_TOKEN_BALANCE,
  UserActionTypes,
} from '../types/user.types';

export interface User {
  authenticated: boolean;
  ethBalance: number | undefined;
  tokenBalance: number | undefined;
  address: string;
  loading: boolean;
}

const initialState: User = {
  authenticated: false,
  ethBalance: undefined,
  tokenBalance: undefined,
  address: '',
  loading: true,
};

const userReducer = (state = { ...initialState }, action: UserActionTypes): User => {
  switch (action.type) {
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
