require('dotenv').config();
const { Contract, Wallet } = require('ethers');
const { JsonRpcProvider } = require('@ethersproject/providers');

const gameManagerContractAddress = '0xE1e72571E96c0a0a9Be33ecaE1a8F7C6dAb13400';
const faucetContractAddress = '0x4697d0CB9E40699237d0f40F3EE211527a5619fF';

const faucetAbi = [
  {
    inputs: [{ internalType: 'address', name: 'token', type: 'address' }],
    stateMutability: 'nonpayable',
    type: 'constructor',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: false,
        internalType: 'address',
        name: 'from',
        type: 'address',
      },
      { indexed: false, internalType: 'address', name: 'to', type: 'address' },
      {
        indexed: false,
        internalType: 'uint256',
        name: 'amount',
        type: 'uint256',
      },
    ],
    name: 'DistributedEth',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: false,
        internalType: 'address',
        name: 'from',
        type: 'address',
      },
      { indexed: false, internalType: 'address', name: 'to', type: 'address' },
      {
        indexed: false,
        internalType: 'uint256',
        name: 'amount',
        type: 'uint256',
      },
    ],
    name: 'DistributedToken',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: false,
        internalType: 'address',
        name: 'from',
        type: 'address',
      },
      { indexed: false, internalType: 'address', name: 'to', type: 'address' },
      {
        indexed: false,
        internalType: 'uint256',
        name: 'amount',
        type: 'uint256',
      },
    ],
    name: 'SentFundsToContract',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      { indexed: false, internalType: 'address', name: 'to', type: 'address' },
      {
        indexed: false,
        internalType: 'uint256',
        name: 'amount',
        type: 'uint256',
      },
    ],
    name: 'WithdrewEthFromContract',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      { indexed: false, internalType: 'address', name: 'to', type: 'address' },
      {
        indexed: false,
        internalType: 'uint256',
        name: 'amount',
        type: 'uint256',
      },
    ],
    name: 'WithdrewTokenFromContract',
    type: 'event',
  },
  {
    inputs: [],
    name: 'acceptOwnership',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      { internalType: 'address', name: 'recipient', type: 'address' },
      { internalType: 'uint256', name: 'value', type: 'uint256' },
    ],
    name: 'distributeEth',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      { internalType: 'address', name: 'recipient', type: 'address' },
      { internalType: 'uint256', name: 'value', type: 'uint256' },
    ],
    name: 'distributeToken',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      { internalType: 'address', name: 'recipient', type: 'address' },
      { internalType: 'uint256', name: 'ethValue', type: 'uint256' },
      { internalType: 'uint256', name: 'tokenValue', type: 'uint256' },
    ],
    name: 'distributeTokenAndEth',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [],
    name: 'owner',
    outputs: [{ internalType: 'address', name: '', type: 'address' }],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [
      { internalType: 'address', name: 'admin_address', type: 'address' },
      { internalType: 'bool', name: 'isAdmin', type: 'bool' },
    ],
    name: 'setAdmin',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [{ internalType: 'address', name: '_newOwner', type: 'address' }],
    name: 'transferOwnership',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [{ internalType: 'address', name: 'to', type: 'address' }],
    name: 'withdrawEth',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [{ internalType: 'address', name: 'to', type: 'address' }],
    name: 'withdrawToken',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  { stateMutability: 'payable', type: 'receive' },
];
const gameManagerAbi = [
  {
    inputs: [
      {
        internalType: 'address',
        name: '_gameToken',
        type: 'address',
      },
      {
        internalType: 'address[]',
        name: '_admins',
        type: 'address[]',
      },
      {
        internalType: 'address[]',
        name: '_permittedGames',
        type: 'address[]',
      },
    ],
    stateMutability: 'nonpayable',
    type: 'constructor',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: true,
        internalType: 'bytes32',
        name: '_roundID',
        type: 'bytes32',
      },
      {
        components: [
          {
            internalType: 'address',
            name: 'playerAddress',
            type: 'address',
          },
          {
            internalType: 'uint256',
            name: 'betAmount',
            type: 'uint256',
          },
          {
            internalType: 'bytes',
            name: 'betData',
            type: 'bytes',
          },
        ],
        indexed: false,
        internalType: 'struct IMultiplayerGameDefinition.Bet',
        name: '_bet',
        type: 'tuple',
      },
    ],
    name: 'Bet',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: false,
        internalType: 'address',
        name: '_gameToken',
        type: 'address',
      },
    ],
    name: 'Constructor',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: true,
        internalType: 'bytes32',
        name: '_roundID',
        type: 'bytes32',
      },
      {
        indexed: true,
        internalType: 'bytes32',
        name: '_persistentGameDataID',
        type: 'bytes32',
      },
      {
        indexed: false,
        internalType: 'bytes32',
        name: 'entropyReveal',
        type: 'bytes32',
      },
      {
        indexed: false,
        internalType: 'address[]',
        name: '_playerAddresses',
        type: 'address[]',
      },
      {
        indexed: false,
        internalType: 'uint256[]',
        name: '_winAmounts',
        type: 'uint256[]',
      },
      {
        indexed: false,
        internalType: 'int256',
        name: '_persistentGameDataPotWinLoss',
        type: 'int256',
      },
      {
        indexed: false,
        internalType: 'bytes',
        name: '_gameResult',
        type: 'bytes',
      },
      {
        indexed: false,
        internalType: 'bytes',
        name: '_historyToRecord',
        type: 'bytes',
      },
    ],
    name: 'EndGameRound',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: true,
        internalType: 'bytes32',
        name: '_roundID',
        type: 'bytes32',
      },
    ],
    name: 'NoMoreBets',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: false,
        internalType: 'address',
        name: '_from',
        type: 'address',
      },
      {
        indexed: false,
        internalType: 'uint256',
        name: '_value',
        type: 'uint256',
      },
      {
        indexed: false,
        internalType: 'bytes',
        name: 'data',
        type: 'bytes',
      },
    ],
    name: 'OnTokenTransfer',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: false,
        internalType: 'enum MultiplayerGamesManager.ContractState',
        name: '_contractState',
        type: 'uint8',
      },
    ],
    name: 'SetContractState',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: false,
        internalType: 'address',
        name: '_gameDefinition',
        type: 'address',
      },
      {
        indexed: false,
        internalType: 'bool',
        name: '_permission',
        type: 'bool',
      },
    ],
    name: 'SetGameDefinitionPermission',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: true,
        internalType: 'bytes32',
        name: '_roundID',
        type: 'bytes32',
      },
      {
        indexed: true,
        internalType: 'bytes32',
        name: '_persistentGameDataID',
        type: 'bytes32',
      },
      {
        components: [
          {
            internalType: 'bytes32',
            name: 'roundID',
            type: 'bytes32',
          },
          {
            internalType: 'address',
            name: 'gameAddress',
            type: 'address',
          },
          {
            internalType: 'bytes32',
            name: 'entropyCommit',
            type: 'bytes32',
          },
          {
            internalType: 'bytes32',
            name: 'persistentGameDataID',
            type: 'bytes32',
          },
        ],
        indexed: false,
        internalType: 'struct MultiplayerGamesManager.GameRoundConfig',
        name: '_config',
        type: 'tuple',
      },
    ],
    name: 'StartGameRound',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: true,
        internalType: 'bytes32',
        name: '_roundID',
        type: 'bytes32',
      },
      {
        indexed: true,
        internalType: 'bytes32',
        name: '_persistentGameDataID',
        type: 'bytes32',
      },
    ],
    name: 'StartGameRoundIds',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: false,
        internalType: 'address',
        name: '_withdrawalAddress',
        type: 'address',
      },
      {
        indexed: false,
        internalType: 'address',
        name: '_tokenAddress',
        type: 'address',
      },
      {
        indexed: false,
        internalType: 'uint256',
        name: '_tokenBalance',
        type: 'uint256',
      },
    ],
    name: 'WithdrawERC20Tokens',
    type: 'event',
  },
  {
    anonymous: false,
    inputs: [
      {
        indexed: false,
        internalType: 'address',
        name: '_withdrawalAddress',
        type: 'address',
      },
      {
        indexed: false,
        internalType: 'uint256',
        name: '_balance',
        type: 'uint256',
      },
      {
        indexed: false,
        internalType: 'uint256',
        name: '_tokenBalance',
        type: 'uint256',
      },
    ],
    name: 'WithdrawHouseFunds',
    type: 'event',
  },
  {
    inputs: [],
    name: 'acceptOwnership',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'address',
        name: '',
        type: 'address',
      },
    ],
    name: 'admins',
    outputs: [
      {
        internalType: 'bool',
        name: '',
        type: 'bool',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'bytes32',
        name: '',
        type: 'bytes32',
      },
    ],
    name: 'consumedEntropyCommits',
    outputs: [
      {
        internalType: 'bool',
        name: '',
        type: 'bool',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [],
    name: 'contractState',
    outputs: [
      {
        internalType: 'enum MultiplayerGamesManager.ContractState',
        name: '',
        type: 'uint8',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'bytes32',
        name: '_roundID',
        type: 'bytes32',
      },
      {
        internalType: 'bytes32',
        name: '_entropyReveal',
        type: 'bytes32',
      },
    ],
    name: 'endGameRound',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'bytes32',
        name: '',
        type: 'bytes32',
      },
    ],
    name: 'gameRounds',
    outputs: [
      {
        components: [
          {
            internalType: 'bytes32',
            name: 'roundID',
            type: 'bytes32',
          },
          {
            internalType: 'address',
            name: 'gameAddress',
            type: 'address',
          },
          {
            internalType: 'bytes32',
            name: 'entropyCommit',
            type: 'bytes32',
          },
          {
            internalType: 'bytes32',
            name: 'persistentGameDataID',
            type: 'bytes32',
          },
        ],
        internalType: 'struct MultiplayerGamesManager.GameRoundConfig',
        name: 'config',
        type: 'tuple',
      },
      {
        internalType: 'enum MultiplayerGamesManager.GameRoundState',
        name: 'state',
        type: 'uint8',
      },
      {
        internalType: 'uint256',
        name: 'persistentGameDataPotValueAtEndRound',
        type: 'uint256',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [],
    name: 'gameToken',
    outputs: [
      {
        internalType: 'contract ERC20Interface',
        name: '',
        type: 'address',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [],
    name: 'getPersistentGameData',
    outputs: [
      {
        internalType: 'uint256',
        name: '_potValue',
        type: 'uint256',
      },
      {
        internalType: 'bytes',
        name: '_gameData',
        type: 'bytes',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'bytes32',
        name: '_roundID',
        type: 'bytes32',
      },
    ],
    name: 'noMoreBetsForGameRound',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [],
    name: 'numRoundsInProgress',
    outputs: [
      {
        internalType: 'uint256',
        name: '',
        type: 'uint256',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'address',
        name: '_from',
        type: 'address',
      },
      {
        internalType: 'uint256',
        name: '_value',
        type: 'uint256',
      },
      {
        internalType: 'bytes',
        name: '_data',
        type: 'bytes',
      },
    ],
    name: 'onTokenTransfer',
    outputs: [
      {
        internalType: 'bool',
        name: '',
        type: 'bool',
      },
    ],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [],
    name: 'owner',
    outputs: [
      {
        internalType: 'address',
        name: '',
        type: 'address',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'address',
        name: '',
        type: 'address',
      },
    ],
    name: 'permittedGameDefinitions',
    outputs: [
      {
        internalType: 'bool',
        name: '',
        type: 'bool',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'bytes32',
        name: '',
        type: 'bytes32',
      },
    ],
    name: 'persistentGameData',
    outputs: [
      {
        internalType: 'address',
        name: 'owner',
        type: 'address',
      },
      {
        internalType: 'address',
        name: 'gameAddress',
        type: 'address',
      },
      {
        internalType: 'uint256',
        name: 'potValue',
        type: 'uint256',
      },
      {
        internalType: 'bytes',
        name: 'gameData',
        type: 'bytes',
      },
    ],
    stateMutability: 'view',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'address',
        name: 'admin_address',
        type: 'address',
      },
      {
        internalType: 'bool',
        name: 'isAdmin',
        type: 'bool',
      },
    ],
    name: 'setAdmin',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'enum MultiplayerGamesManager.ContractState',
        name: '_contractState',
        type: 'uint8',
      },
    ],
    name: 'setContractState',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'address',
        name: '_gameDefinition',
        type: 'address',
      },
      {
        internalType: 'bool',
        name: '_permission',
        type: 'bool',
      },
    ],
    name: 'setGameDefinitionPermission',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'bytes32',
        name: 'roundID',
        type: 'bytes32',
      },
      {
        internalType: 'address',
        name: 'gameAddress',
        type: 'address',
      },
      {
        internalType: 'bytes32',
        name: 'entropyCommit',
        type: 'bytes32',
      },
    ],
    name: 'startGameRoundSplit',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'address',
        name: '_newOwner',
        type: 'address',
      },
    ],
    name: 'transferOwnership',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
  {
    inputs: [
      {
        internalType: 'address payable',
        name: '_withdrawalAddress',
        type: 'address',
      },
      {
        internalType: 'address',
        name: 'tokenAddress',
        type: 'address',
      },
    ],
    name: 'withdrawERC20Tokens',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function',
  },
];

const getL2Wallet = () => {
  const l2RpcProvider = new JsonRpcProvider('https://kovan.optimism.io');
  return new Wallet(process.env.YOUR_PRIVATE_KEY, l2RpcProvider);
};

const getGameManagerContract = () => {
  return new Contract(
    gameManagerContractAddress,
    gameManagerAbi,
    getL2Wallet()
  );
};

const getFaucetContract = () => {
  return new Contract(faucetContractAddress, faucetAbi, getL2Wallet());
};

module.exports = { getGameManagerContract, getFaucetContract };
