import { BlockHeader } from '../../model/blockHeader';
import { LRError } from '../../model/errorCodes';
import {
  SET_NETWORK_ID,
  SET_NETWORK_NAME,
  SET_BLOCK_HEADER,
  NetworkActionTypes,
  CLEAR_NETWORK_STATE,
  SET_TOKEN_SYMBOL,
  SET_TRANSACTION_HASH,
  SET_NETWORK_ERROR,
} from '../types/network.types';

export interface Network {
  error: LRError | null;
  id: number | null;
  name: string | null;
  blockHeader: BlockHeader | null;
  tokenSymbol: string | null;
  transactionHash: string | null;
}

const initialState: Network = {
  error: null,
  id: null,
  name: null,
  blockHeader: null,
  tokenSymbol: null,
  transactionHash: null,
};

const networkReducer = (state = { ...initialState }, action: NetworkActionTypes ): Network => {
  switch (action.type) {
    case CLEAR_NETWORK_STATE:
      return initialState;
    case SET_NETWORK_ERROR:
      return {
        ...state,
        error: action.payload,
      };
    case SET_NETWORK_ID:
      return {
        ...state,
        id: action.payload,
      };
    case SET_TOKEN_SYMBOL:
      return {
        ...state,
        tokenSymbol: action.payload,
      };
    case SET_NETWORK_NAME:
      return {
        ...state,
        name: action.payload,
      };
    case SET_BLOCK_HEADER:
      return {
        ...state,
        blockHeader: action.payload,
      };
    case SET_TRANSACTION_HASH:
      return {
        ...state,
        transactionHash: action.payload,
      };
    default: {
      return state;
    }
  }
};

export default networkReducer;
