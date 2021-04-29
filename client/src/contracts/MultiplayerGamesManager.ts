import {
  ContractTransaction,
  ContractInterface,
  BytesLike as Arrayish,
  BigNumber,
  BigNumberish,
} from 'ethers';
import { EthersContractContextV5 } from 'ethereum-abi-types-generator';

export type ContractContext = EthersContractContextV5<
  MultiplayerGamesManager,
  MultiplayerGamesManagerMethodNames,
  MultiplayerGamesManagerEventsContext,
  MultiplayerGamesManagerEvents
>;

export declare type EventFilter = {
  address?: string;
  topics?: Array<string>;
  fromBlock?: string | number;
  toBlock?: string | number;
};

export interface ContractTransactionOverrides {
  /**
   * The maximum units of gas for the transaction to use
   */
  gasLimit?: number;
  /**
   * The price (in wei) per unit of gas
   */
  gasPrice?: BigNumber | string | number | Promise<any>;
  /**
   * The nonce to use in the transaction
   */
  nonce?: number;
  /**
   * The amount to send with the transaction (i.e. msg.value)
   */
  value?: BigNumber | string | number | Promise<any>;
  /**
   * The chain ID (or network ID) to use
   */
  chainId?: number;
}

export interface ContractCallOverrides {
  /**
   * The address to execute the call as
   */
  from?: string;
  /**
   * The maximum units of gas for the transaction to use
   */
  gasLimit?: number;
}
export type MultiplayerGamesManagerEvents =
  | 'Bet'
  | 'Constructor'
  | 'EndGameRound'
  | 'NoMoreBets'
  | 'OnTokenTransfer'
  | 'SetContractState'
  | 'SetGameDefinitionPermission'
  | 'StartGameRound'
  | 'StartGameRoundIds'
  | 'WithdrawERC20Tokens'
  | 'WithdrawHouseFunds';
export interface MultiplayerGamesManagerEventsContext {
  Bet(...parameters: any): EventFilter;
  Constructor(...parameters: any): EventFilter;
  EndGameRound(...parameters: any): EventFilter;
  NoMoreBets(...parameters: any): EventFilter;
  OnTokenTransfer(...parameters: any): EventFilter;
  SetContractState(...parameters: any): EventFilter;
  SetGameDefinitionPermission(...parameters: any): EventFilter;
  StartGameRound(...parameters: any): EventFilter;
  StartGameRoundIds(...parameters: any): EventFilter;
  WithdrawERC20Tokens(...parameters: any): EventFilter;
  WithdrawHouseFunds(...parameters: any): EventFilter;
}
export type MultiplayerGamesManagerMethodNames =
  | 'new'
  | 'acceptOwnership'
  | 'admins'
  | 'consumedEntropyCommits'
  | 'contractState'
  | 'endGameRound'
  | 'gameRounds'
  | 'gameToken'
  | 'getPersistentGameData'
  | 'getPersistentGameDataByID'
  | 'noMoreBetsForGameRound'
  | 'numRoundsInProgress'
  | 'onTokenTransfer'
  | 'owner'
  | 'permittedGameDefinitions'
  | 'persistentGameData'
  | 'setAdmin'
  | 'setContractState'
  | 'setGameDefinitionPermission'
  | 'startGameRoundSplit'
  | 'transferOwnership'
  | 'withdrawERC20Tokens';
export interface ConfigResponse {
  roundID: string;
  0: string;
  gameAddress: string;
  1: string;
  entropyCommit: string;
  2: string;
  persistentGameDataID: string;
  3: string;
}
export interface GameRoundsResponse {
  config: ConfigResponse;
  0: ConfigResponse;
  state: number;
  1: number;
  persistentGameDataPotValueAtEndRound: BigNumber;
  2: BigNumber;
  length: 3;
}
export interface GetPersistentGameDataResponse {
  _potValue: BigNumber;
  0: BigNumber;
  _gameData: string;
  1: string;
  length: 2;
}
export interface GetPersistentGameDataByIDResponse {
  _potValue: BigNumber;
  0: BigNumber;
  _gameData: string;
  1: string;
  length: 2;
}
export interface PersistentGameDataResponse {
  owner: string;
  0: string;
  gameAddress: string;
  1: string;
  potValue: BigNumber;
  2: BigNumber;
  gameData: string;
  3: string;
  length: 4;
}
export interface MultiplayerGamesManager {
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: constructor
   * @param _gameToken Type: address, Indexed: false
   * @param _admins Type: address[], Indexed: false
   * @param _permittedGames Type: address[], Indexed: false
   */
  'new'(
    _gameToken: string,
    _admins: string[],
    _permittedGames: string[],
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   */
  acceptOwnership(
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   * @param parameter0 Type: address, Indexed: false
   */
  admins(
    parameter0: string,
    overrides?: ContractCallOverrides
  ): Promise<boolean>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   * @param parameter0 Type: bytes32, Indexed: false
   */
  consumedEntropyCommits(
    parameter0: Arrayish,
    overrides?: ContractCallOverrides
  ): Promise<boolean>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   */
  contractState(overrides?: ContractCallOverrides): Promise<number>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   * @param _roundID Type: bytes32, Indexed: false
   * @param _entropyReveal Type: bytes32, Indexed: false
   */
  endGameRound(
    _roundID: Arrayish,
    _entropyReveal: Arrayish,
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   * @param parameter0 Type: bytes32, Indexed: false
   */
  gameRounds(
    parameter0: Arrayish,
    overrides?: ContractCallOverrides
  ): Promise<GameRoundsResponse>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   */
  gameToken(overrides?: ContractCallOverrides): Promise<string>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   */
  getPersistentGameData(
    overrides?: ContractCallOverrides
  ): Promise<GetPersistentGameDataResponse>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   * @param _gameDataID Type: bytes32, Indexed: false
   */
  getPersistentGameDataByID(
    _gameDataID: Arrayish,
    overrides?: ContractCallOverrides
  ): Promise<GetPersistentGameDataByIDResponse>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   * @param _roundID Type: bytes32, Indexed: false
   */
  noMoreBetsForGameRound(
    _roundID: Arrayish,
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   */
  numRoundsInProgress(overrides?: ContractCallOverrides): Promise<BigNumber>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   * @param _from Type: address, Indexed: false
   * @param _value Type: uint256, Indexed: false
   * @param _data Type: bytes, Indexed: false
   */
  onTokenTransfer(
    _from: string,
    _value: BigNumberish,
    _data: Arrayish,
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   */
  owner(overrides?: ContractCallOverrides): Promise<string>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   * @param parameter0 Type: address, Indexed: false
   */
  permittedGameDefinitions(
    parameter0: string,
    overrides?: ContractCallOverrides
  ): Promise<boolean>;
  /**
   * Payable: false
   * Constant: true
   * StateMutability: view
   * Type: function
   * @param parameter0 Type: bytes32, Indexed: false
   */
  persistentGameData(
    parameter0: Arrayish,
    overrides?: ContractCallOverrides
  ): Promise<PersistentGameDataResponse>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   * @param admin_address Type: address, Indexed: false
   * @param isAdmin Type: bool, Indexed: false
   */
  setAdmin(
    admin_address: string,
    isAdmin: boolean,
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   * @param _contractState Type: uint8, Indexed: false
   */
  setContractState(
    _contractState: BigNumberish,
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   * @param _gameDefinition Type: address, Indexed: false
   * @param _permission Type: bool, Indexed: false
   */
  setGameDefinitionPermission(
    _gameDefinition: string,
    _permission: boolean,
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   * @param roundID Type: bytes32, Indexed: false
   * @param gameAddress Type: address, Indexed: false
   * @param entropyCommit Type: bytes32, Indexed: false
   */
  startGameRoundSplit(
    roundID: Arrayish,
    gameAddress: string,
    entropyCommit: Arrayish,
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   * @param _newOwner Type: address, Indexed: false
   */
  transferOwnership(
    _newOwner: string,
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
  /**
   * Payable: false
   * Constant: false
   * StateMutability: nonpayable
   * Type: function
   * @param _withdrawalAddress Type: address, Indexed: false
   * @param tokenAddress Type: address, Indexed: false
   */
  withdrawERC20Tokens(
    _withdrawalAddress: string,
    tokenAddress: string,
    overrides?: ContractTransactionOverrides
  ): Promise<ContractTransaction>;
}
