import { FFEngine } from '@funfair/engine';

/**
 * Types of font asset supported by this asset pack
 */
 export enum FontAssetType {
    STANDARD,
}

/**
 * Game asset loader and manager
 */
export class AssetPack {

    private fontAssets: any[] = [];

    static Create(): void {
        ASSETPACK = new AssetPack();
    }

    constructor() {

        //Font assets
        FFEngine.instance.assetLoader.LoadFont('game/font_en.fnt', (data: any) => {
            this.fontAssets[FontAssetType.STANDARD] = data;
        });
        
    }
}

/**
 * Global Singleton reference to the asset pack
 */
 let ASSETPACK!: AssetPack;
 export { ASSETPACK };
