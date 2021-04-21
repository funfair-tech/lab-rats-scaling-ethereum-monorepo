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

        //Load Font assets
        FFEngine.instance.assetLoader.LoadFont('game/font_en.fnt', (data: any) => {
            this.fontAssets[FontAssetType.STANDARD] = data;
        });
        
    }

    /**
     * Returns the corresponding loaded font asset
     */
     public GetFontAsset(type: FontAssetType): any {
        return this.fontAssets[type];
    }
}

/**
 * Global Singleton reference to the asset pack
 */
 let ASSETPACK!: AssetPack;
 export { ASSETPACK };
