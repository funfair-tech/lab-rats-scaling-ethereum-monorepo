import { FFEngine } from '@funfair/engine';
import { Bet } from '../model/bet';
import { MultiTrader } from './multiTrader';
import { RoundResult } from '../model/roundResult';
import { Round } from '../model/round';

/**
 * Main game entry point, for initing the game engine and loading the game
 */
export class Game {

  constructor(private play: (bet: Bet) => void) {};

  public handleNextRound(round: Round): void {
    // TODO: ...
    // anotherGame.getLogic.apiMessgeCall(typeString, round)
  }

  public handleNoMoreBets(roundID: string): void {
    // TODO: ...
  }

  public handleRoundResult(roundResult: RoundResult): void {
    // TODO: ...
    // console.log('round result in game: ', roundResult);
  }

  public handleBet(bet: Bet): void {
    // TODO: ...
  }

  public setAddress(address: string): void {
    console.log('Game: user address -', address);
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