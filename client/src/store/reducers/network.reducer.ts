import { BlockHeader } from '../../model/blockHeader';
import { SET_NETWORK_ID, SET_NETWORK_NAME, SET_BLOCK_HEADER, NetworkActionTypes, CLEAR_NETWORK_STATE, SET_TOKEN_SYMBOL } from '../types/network.types';

export interface Network {
  id: number|null; 
  name: string|null; 
  blockHeader: BlockHeader | null;
  tokenSymbol: string | null;
}

const initialState: Network = {
  id: null, 
  name: null, 
  blockHeader: null,
  tokenSymbol: null,
};

const networkReducer = (state = {...initialState}, action: NetworkActionTypes): Network => {
  switch (action.type) {
    case CLEAR_NETWORK_STATE:
      return initialState;
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
    default: {
      return state;
    }
  }
}

export default networkReducer;
