import { GameData } from './gameData';

export interface RoundResult {
  id: string;
  playerAddresses: string[];
  winAmounts: number[];
  result: string;
  history: string;
  potWinLoss: string;
  entropyReveal: string;
  persistentGameData: GameData|null; 
}
