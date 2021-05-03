import { BlockHeader } from '../../model/blockHeader';
import { LRError } from '../../model/errorCodes';

export const SET_NETWORK_ERROR = 'SET_NETWORK_ERROR';
export const SET_NETWORK_ID = 'SET_NETWORK_ID';
export const SET_TOKEN_SYMBOL = 'SET_TOKEN_SYMBOL';
export const SET_NETWORK_NAME = 'SET_NETWORK_NAME';
export const SET_BLOCK_HEADER = 'SET_BLOCK_HEADER';
export const CLEAR_NETWORK_STATE = 'CLEAR_NETWORK_STATE';
export const SET_TRANSACTION_HASH = 'SET_TRANSACTION_HASH';

export interface SetNetworkError {
  type: typeof SET_NETWORK_ERROR;
  payload: LRError | null;
}

export interface SetNetworkId {
  type: typeof SET_NETWORK_ID;
  payload: number;
}

export interface SetTokenSymbol {
  type: typeof SET_TOKEN_SYMBOL;
  payload: string;
}

export interface SetNetworkName {
  type: typeof SET_NETWORK_NAME;
  payload: string;
}

export interface SetBlockHeader {
  type: typeof SET_BLOCK_HEADER;
  payload: BlockHeader;
}

export interface ClearNetworkStaate {
  type: typeof CLEAR_NETWORK_STATE;
}

export interface SetTransactionHash {
  type: typeof SET_TRANSACTION_HASH;
  payload: string | null;
}

export type NetworkActionTypes =
  | SetNetworkError
  | SetNetworkId
  | SetTokenSymbol
  | SetNetworkName
  | SetBlockHeader
  | ClearNetworkStaate
  | SetTransactionHash;
