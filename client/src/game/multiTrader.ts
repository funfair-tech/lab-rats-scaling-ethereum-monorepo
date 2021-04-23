import { FFEngine } from '@funfair/engine';
import { AssetPack } from './assetPack';
import { GLUI, MultiTraderUI } from './multiTraderUI';
import { EnvironmentManager, ENVIRONMENT_MANAGER } from './objectManagers/environmentManager';

import { LOGIC, Logic } from './logic/logic';
import { LOGIC_TESTCODE } from './logic/logic_testcode';
import { Logic_Configuration } from './logic/logic_defines';

/**
 * Main game scene for the multiplayer trader game
 */
export class MultiTrader extends FFEngine.Component {

    private startupFinished: boolean = false;

    public Create(params: any): void {

        //assign global singleton
        MULTITRADER = this;

        //create game scene container
        this.container = new FFEngine.THREE.Object3D();

        //create asset manager and begin asset load
        AssetPack.Create();

        //Create Game Components
        FFEngine.instance.CreateChildObjectWithComponent(this.container, EnvironmentManager);
        MultiTraderUI.SetupScene();

        //Asset loading callback
        FFEngine.instance.assetLoader.AddLoadingPhaseCompleteCallback(() => this.AssetLoadingFinished());

        Logic.Create(new Logic_Configuration(1000, 10, 5, 100, 30), true);
    }

    public Update(): void {
        this.UpdateLoadingPhase();
        LOGIC.Tick();
        //console.log('paul this is the gamestate - watch and it will change over time... ' + JSON.stringify(LOGIC.GetCurrentState()));
    }

    public OnKeyUp(params: any): void {
        super.OnKeyUp(params);

        if (this.startupFinished === true) {
            if (FFEngine.instance.debugBuild) {
                if (params.keyCode === 68) { // 'D'
                    FFEngine.instance.ToggleDebugDisplay();
                }
            }
        }
    }

    /**
     * Callback from engine for when all assets have finished loading
     */
    public AssetLoadingFinished(): void {
        ENVIRONMENT_MANAGER.AssetLoadingFinished();
        GLUI.AssetLoadingFinished();

        //update and resize initial canvas and display
        let options = new FFEngine.PlatformOptions();
        options.allowPortrait = true;
        options.aspectRatio = 0;
        options.canvasType = FFEngine.CanvasType.STANDALONE;
        FFEngine.instance.SetPlatformOptions(options);
    }

    private UpdateLoadingPhase(): void {
        if (FFEngine.instance.assetLoader.IsLoadingPhaseActive()) {
            let assetLoadCoef = FFEngine.instance.assetLoader.GetAssetLoadingCoef();
    
            //update loading bar here if we want one

            //check for load phase finish
            if (assetLoadCoef >= 1) {
                FFEngine.instance.assetLoader.EndLoadingPhase();
                this.startupFinished = true;
            }
        }
    }

}

/**
 * Global Singleton reference to the asset pack
 */
 let MULTITRADER!: MultiTrader;
 export { MULTITRADER };
