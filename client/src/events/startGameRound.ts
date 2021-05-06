export enum StartGameRound {
  ROUND_ID = 0,
  PERSISTENT_GAME_DATA_ID = 1,  
  CONFIG = 2, 
}

export interface IStartGameRound {
  entropyCommit: string;
  gameAddress: string;
  persistentGameDataID: string;
  roundID: string;
}

