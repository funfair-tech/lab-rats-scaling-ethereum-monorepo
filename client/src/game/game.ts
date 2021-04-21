import { FFEngine } from '@funfair/engine';
import { MultiTrader } from './multiTrader';

/**
 * Main game entry point, for initing the game engine and loading the game
 */
export class Game {

  constructor() {
    console.log('Creating Labrats Game');
    //this.loadGame(); //this does not work, DOM not ready
  }

  public report(): string {
    return 'Canvas ready for duty';
  }

  public getEngineVersion(): string {
    return FFEngine.instance.GetVersion();
  }

  public initEngine(): void {
    console.log('Engine: ', FFEngine);
    //FFEngine.instance.Init();
    this.loadGame();
  }

  private loadGame(): void {
    // Define the game scene data
    let sceneData = new FFEngine.SceneData();
    sceneData.sceneClass = MultiTrader;
    sceneData.isLocalGame = true;

    //set config data keys
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