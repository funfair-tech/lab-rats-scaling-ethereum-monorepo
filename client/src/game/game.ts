// import { FFEngine } from '@funfair/engine';
const Engine = require('@funfair/engine');

export class Game {
  public report(): string {
    return 'Canvas ready for duty';
  }

  public getEngineVersion(): string {
    console.log('Engine: ', Engine);
    return Engine.FFEngine.instance.GetVersion();
  }

  public initEngine(): void {
    console.log('Engine: ', Engine);
    Engine.FFEngine.instance.Init();
  }
}