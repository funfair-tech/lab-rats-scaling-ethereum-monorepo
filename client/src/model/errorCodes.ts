export enum ErrorCode {
  GENERAL_BET_ERROR = 0,
  LOW_BALANCE_BET_ERROR = 1,
  TIMEOUT_BET_ERROR = 2,
  FAUCET_ERROR = 3,
  JSON_RPC_READ_ERROR = 4,
  JSON_RPC_WRITE_ERROR = 5,
}

export interface LRError {
  code: ErrorCode;
  msg: string;
}