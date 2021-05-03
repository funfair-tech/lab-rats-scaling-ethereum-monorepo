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

export interface User {
  error: LRError | null;
  authenticated: boolean;
  ethBalance: number | null;
  tokenBalance: number | null;
  displayBalance: number | null;
  displayBalanceFrozen: boolean;
  address: string;
  loading: boolean;
}

const initialState: User = {
  error: null,
  authenticated: false,
  ethBalance: null,
  tokenBalance: null,
  displayBalance: null,
  displayBalanceFrozen: false,
  address: '',
  loading: true,
};

const userReducer = (
  state = { ...initialState },
  action: UserActionTypes
): User => {
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
        displayBalance: state.displayBalanceFrozen ? state.displayBalance : action.payload,
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
    case FREEZE_DISPLAY_BALANCE:
      return {
        ...state,
        displayBalanceFrozen: true,
      };
    case UNFREEZE_DISPLAY_BALANCE:
      return {
        ...state,
        displayBalanceFrozen: false,
        displayBalance: state.tokenBalance,
      };
    default: {
      return state;
    }
  }
};

export default userReducer;
