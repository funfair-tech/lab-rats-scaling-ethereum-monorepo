export enum BetEvent {
  ROUND_ID = 0,
  DATA = 1,  
}

export interface IBet {
  roundId: string;
  data: {
    playerAddress: string;
    betAmount: number;
    betData: string;
  };  
}

