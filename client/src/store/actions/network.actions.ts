import { BlockHeader } from '../../model/blockHeader';
import {
  CLEAR_NETWORK_STATE,
  NetworkActionTypes,
  SET_BLOCK_HEADER,
  SET_NETWORK_ID,
  SET_NETWORK_NAME,
  SET_TOKEN_SYMBOL,
  SET_TRANSACTION_HASH,
} from '../types/network.types';

export const setNetworkId = (id: number): NetworkActionTypes => {
  return {
    type: SET_NETWORK_ID,
    payload: id,
  };
};

export const setTokenSymbol = (tokenSymbol: string): NetworkActionTypes => {
  return {
    type: SET_TOKEN_SYMBOL,
    payload: tokenSymbol,
  };
};

export const setNetworkName = (name: string): NetworkActionTypes => {
  return {
    type: SET_NETWORK_NAME,
    payload: name,
  };
};

export const setBlockHeader = (blockHeader: BlockHeader): NetworkActionTypes => {
  return {
    type: SET_BLOCK_HEADER,
    payload: blockHeader,
  };
};

export const clearNetworkState = (): NetworkActionTypes => {
  return {
    type: CLEAR_NETWORK_STATE,
  };
};

export const setTransactionHash = (hash: string | null): NetworkActionTypes => {
  return {
    type: SET_TRANSACTION_HASH,
    payload: hash,
  };
};
