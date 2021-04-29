import { FFEngine } from '@funfair/engine';
import { Bet } from '../model/bet';
import { MultiTrader } from './multiTrader';
import { RoundResult } from '../model/roundResult';
import { Round } from '../model/round';
import { GameEvent } from '../events/gameEvent';
import { LOGIC_SERVERFEEDQUEUE } from './logic/logic_serverfeedqueue';

/**
 * Main game entry point, for initing the game engine and loading the game
 */
export class Game {

  constructor(private play: (stake: number, action: number) => void) {};

  public handleNextRound(round: Round): void {
    console.log('++ new round ', round);
    LOGIC_SERVERFEEDQUEUE.APIMessageCall(GameEvent.NEW_ROUND, round);
  }

  public handleNoMoreBets(roundId: string): void {
    console.log('++ no more bets on round ', roundId);
    LOGIC_SERVERFEEDQUEUE.APIMessageCall(GameEvent.NO_MORE_BETS, roundId);
  }

  public handleRoundResult(roundResult: RoundResult): void {
    console.log('++ round result ', roundResult);
    LOGIC_SERVERFEEDQUEUE.APIMessageCall(GameEvent.RESULT, roundResult);
  }

  public handleBets(bets: Bet[]): void {
    console.log('++ bets  ', bets);
    LOGIC_SERVERFEEDQUEUE.APIMessageCall(GameEvent.BETS, bets);
  }

  public setAddress(address: string): void {
    console.log('Game: user address -', address);
    LOGIC_SERVERFEEDQUEUE.APIMessageCall('LOCALPLAYERADDRESS', address);
    MultiTrader.SetPlayerAddress(address);
  }

  public report(): string {
    return 'Canvas ready for duty';
  }

  public getEngineVersion(): string {
    return FFEngine.instance.GetVersion();
  }

  public initEngine(): void {
    this.loadGame();
  }

  private loadGame(): void {

    // Define the game scene data
    let sceneData = new FFEngine.SceneData();
    sceneData.sceneClass = MultiTrader;
    sceneData.isLocalGame = true;

    //set save data keys
    FFEngine.EngineConfig.SetDataKey('labrats-game');
    
    // Start the Engine
    let options = new FFEngine.PlatformOptions();
    options.allowPortrait = true;
    options.aspectRatio = 0;
    options.canvasType = FFEngine.CanvasType.STANDALONE;
    FFEngine.instance.Init(options, true);

    // Load the game scene
    FFEngine.instance.LoadScene(sceneData);
  }
}