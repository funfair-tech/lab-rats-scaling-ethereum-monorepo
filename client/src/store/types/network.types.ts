import { BlockHeader } from '../../model/blockHeader';

export const SET_NETWORK_ID = 'SET_NETWORK_ID';
export const SET_TOKEN_SYMBOL = 'SET_TOKEN_SYMBOL';
export const SET_NETWORK_NAME = 'SET_NETWORK_NAME';
export const SET_BLOCK_HEADER = 'SET_BLOCK_HEADER';
export const CLEAR_NETWORK_STATE = 'CLEAR_NETWORK_STATE';

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

export type NetworkActionTypes = SetNetworkId | SetTokenSymbol | SetNetworkName | SetBlockHeader | ClearNetworkStaate;
