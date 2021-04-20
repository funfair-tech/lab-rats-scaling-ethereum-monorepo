// import { FFEngine } from '@funfair/engine';
const FFEngine = require('@funfair/engine').FFEngine;

export class Game {

  constructor() {
    console.log('Creating Labrats Game');
  }

  public report(): string {
    return 'Canvas ready for duty';
  }

  public getEngineVersion(): string {
    console.log('Engine: ', FFEngine);
    return FFEngine.instance.GetVersion();
  }

  public initEngine(): void {
    console.log('Engine: ', FFEngine);
    FFEngine.instance.Init();
  }
}