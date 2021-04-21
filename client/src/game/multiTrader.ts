import { FFEngine } from '@funfair/engine';
import { AssetPack } from './assetPack';
import { EnvironmentManager, ENVIRONMENT_MANAGER } from './objectManagers/environmentManager';

/**
 * Main game scene for the multiplayer trader game
 */
export class MultiTrader extends FFEngine.Component {

    public Create(params: any): void {

        //create game scene container
        this.container = new FFEngine.THREE.Object3D();

        //create asset manager and begin asset load
        AssetPack.Create();

        //Create Game Components
        FFEngine.instance.CreateChildObjectWithComponent(this.container, EnvironmentManager);

        //Asset loading callback
        FFEngine.instance.assetLoader.AddLoadingPhaseCompleteCallback(() => this.AssetLoadingFinished());
    }

    public Update(): void {
        this.UpdateLoadingPhase();
    }

    /**
     * Callback from engine for when all assets have finished loading
     */
    public AssetLoadingFinished(): void {
        ENVIRONMENT_MANAGER.AssetLoadingFinished();
    }

    private UpdateLoadingPhase(): void {
        if (FFEngine.instance.assetLoader.IsLoadingPhaseActive()) {
            let assetLoadCoef = FFEngine.instance.assetLoader.GetAssetLoadingCoef();
    
            //update loading bar here if we want one

            //check for load phase finish
            if (assetLoadCoef >= 1) {
                FFEngine.instance.assetLoader.EndLoadingPhase();
            }
        }
    }

}
