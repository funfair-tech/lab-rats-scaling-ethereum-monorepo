import { FFEngine } from '@funfair/engine';
import { AssetPack } from './assetPack';
import { EnvironmentManager } from './objectManagers/environmentManager';

/**
 * Main game scene for the multiplayer trader game
 */
export class MultiTrader extends FFEngine.Component {

    private sprite!: FFEngine.Sprite;
    
    public Create(params: any): void {

        //create game scene container
        this.container = new FFEngine.THREE.Object3D();

        //create asset manager and begin asset load
        AssetPack.create();

        //Create Game Components
        FFEngine.instance.CreateChildObjectWithComponent(this.container, EnvironmentManager);

        //test
        this.sprite = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        this.sprite.SetSize(5, 5);
    }

    public OnRendererResize(params: FFEngine.IRendererResizeParams): void {
        super.OnRendererResize(params);

        console.log('Game Resized to: ' + params.width + ',' + params.height);
    }

}
