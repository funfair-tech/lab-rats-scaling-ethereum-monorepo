// import { FFEngine } from '@funfair/engine';
const Engine = require('@funfair/engine');

export class Game {
  public report(): string {
    return 'Canvas ready for duty';
  }

  public getEngineVersion(): string {
    return !!Engine.FFEngine ? Engine.FFEngine.instance.GetVersion() : '';
  }

  public initEngine(): void {
    Engine.FFEngine.instance.Init();
  }
}