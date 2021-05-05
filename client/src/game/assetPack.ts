import { FFEngine } from '@funfair/engine';

/**
 * Types of font asset supported by this asset pack
 */
 export enum FontAssetType {
    STANDARD,
}

/**
 * Types of texture asset supported by this asset pack
 */
export enum TextureAssetType {
    LINE,
    GLOW,
    CELL,
    BUTTONBETHIGH,
    BUTTONBETLOW
}

/**
 * Game asset loader and manager
 */
export class AssetPack {

    private fontAssets: any[] = [];
    private textureAssets: FFEngine.THREE.Texture[] = [];

    static Create(): void {
        ASSETPACK = new AssetPack();
    }

    constructor() {

        //load font assets
        FFEngine.instance.assetLoader.LoadFont('game/font_en.fnt', (data: any) => {
            this.fontAssets[FontAssetType.STANDARD] = data;
        });

        //load texture assets
        FFEngine.instance.assetLoader.LoadTextureNoMipMaps('game/line.png', (texture: any) => {
            this.textureAssets[TextureAssetType.LINE] = texture;
        });

        FFEngine.instance.assetLoader.LoadTextureNoMipMaps('game/glow.png', (texture: any) => {
            this.textureAssets[TextureAssetType.GLOW] = texture;
        });

        FFEngine.instance.assetLoader.LoadTextureNoMipMaps('game/cell.png', (texture: any) => {
            this.textureAssets[TextureAssetType.CELL] = texture;
        });
        
        FFEngine.instance.assetLoader.LoadTextureNoMipMaps('game/bethigh.png', (texture: any) => {
            this.textureAssets[TextureAssetType.BUTTONBETHIGH] = texture;
        });

        FFEngine.instance.assetLoader.LoadTextureNoMipMaps('game/betlow.png', (texture: any) => {
            this.textureAssets[TextureAssetType.BUTTONBETLOW] = texture;
        });
    }

    /**
     * Returns the corresponding loaded font asset
     */
    public GetFontAsset(type: FontAssetType): any {
        return this.fontAssets[type];
    }

    /**
     * Returns the corresponding loaded texture asset
     */
    public GetTextureAsset(type: TextureAssetType): FFEngine.THREE.Texture {
        return this.textureAssets[type];
    }
}

/**
 * Global Singleton reference to the asset pack
 */
 let ASSETPACK!: AssetPack;
 export { ASSETPACK };
