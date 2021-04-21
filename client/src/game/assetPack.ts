import { FFEngine } from '@funfair/engine';

/**
 * Game asset loader and manager
 */
export class AssetPack {

    static create(): void {
        ASSETPACK = new AssetPack();
    }

    constructor() {

        //todo: load assets
        
    }
}

/**
 * Global Singleton reference to the asset pack
 */
 let ASSETPACK!: AssetPack;
 export { ASSETPACK };
