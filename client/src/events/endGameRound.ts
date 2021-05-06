export enum EndGameRound {
  ROUND_ID = 0,
  PERSISTENT_GAME_DATA_ID = 1,  
  ENTROPY_REVEAL = 2, 
  PLAYER_ADDRESSES = 3, 
  WIN_AMOUNTS = 4, 
  POT_WIN_LOSS = 5, 
  GAME_RESULT = 6, 
  HISTORY = 7,
}

export interface IEndGameRound {
  roundId: string;  
  persistentGameDataID: string; 
  entropyReveal: string; 
  playerAddresses: string[]; 
  winAmounts: number[]; 
  persistentGameDataPotWinLoss: string; 
  result: string; 
  history: string;
}

