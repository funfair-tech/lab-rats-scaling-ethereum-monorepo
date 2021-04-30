import { FFEngine } from '@funfair/engine';
import { AssetPack } from './assetPack';
import { GLUI, MultiTraderUI } from './multiTraderUI';
import { EnvironmentManager, ENVIRONMENT_MANAGER } from './objectManagers/environmentManager';

import { LOGIC, Logic } from './logic/logic';
import { LOGIC_TESTCODE } from './logic/logic_testcode';
import { Logic_BetType, Logic_Configuration, Logic_RoundState } from './logic/logic_defines';
import { GRAPH_MANAGER } from './objectManagers/graphManager';
import { PLAYER_MANAGER } from './objectManagers/playerManager';
import { UIPlayerDisplayMode } from './objects/uiPlayerList';

/**
 * Main game scene for the multiplayer trader game
 */
export class MultiTrader extends FFEngine.Component {

    private static playerAddress: string = '0x00000000';

    private startupFinished: boolean = false;
    private gamePhase: Logic_RoundState = Logic_RoundState.NOTSTARTED;
    private lastNonce: number = 1;

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

        if (this.startupFinished === true) {
            this.UpdateLogicState();
        }
    }

    public OnKeyUp(params: any): void {
        super.OnKeyUp(params);
        if (this.startupFinished === true) {
            if (FFEngine.instance.debugBuild) {
                if (params.keyCode === 68) { // 'D'
                    FFEngine.instance.ToggleDebugDisplay();
                }

                //test adding graph results
                if (params.keyCode === 69) { // 'E'
                    GRAPH_MANAGER.AddResult(FFEngine.MathHelper.GetRandomRange(-2, 2));
                }

                //Reserve some keys for testlogic
                // 0 - 9 inclusive
                //q,w,r,t,y
                if (((params.keyCode >= 48) && (params.keyCode <= 57)) ||
                (params.keyCode === 81) ||
                (params.keyCode === 87) ||
                (params.keyCode === 82) ||
                (params.keyCode === 84) ||
                (params.keyCode === 89) 
                ) {
                    LOGIC_TESTCODE.KeyPressed(params.keyCode);
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

    /**
     * Sets the local player's account address
     */
    public static SetPlayerAddress(address: string): void {
        MultiTrader.playerAddress = address;
    }

    /**
     * Sends a bet to the logic
     */
    public InitiatePlayerBet(betType: Logic_BetType): void {
        let betResponse = LOGIC.PlaceBetForLocalPlayer(MultiTrader.playerAddress, betType);
        console.log('bet response: ' + betResponse);
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

    private UpdateLogicState(): void {
        LOGIC.Tick();
        let state = LOGIC.GetCurrentState();
        this.SetGamePhase(state.roundState);

        //advance state if necessary
        if (this.lastNonce !== state.localNonce) {

            if (this.lastNonce === 1) {
                //add all historical items starting with the oldest
                for (let i=state.historicPrices.length-1;i>=0;i--) {
                    GRAPH_MANAGER.AddResult(state.historicPrices[i]);
                }
                GRAPH_MANAGER.AddResult(state.currentPrice);
            }

            this.lastNonce = state.localNonce;

            //add a new graph result
            if (state.roundState === Logic_RoundState.COMPLETE) {
                GRAPH_MANAGER.AddResult(state.currentPrice);
            }

            //update prize pool
            GLUI.SetPrizePool(state.currentPrizePool);

            //update bets and players
            PLAYER_MANAGER.UpdateBets(state.bets);
        }
        
    }

    /**
     * State Machine handles changes of state to the game display at any time
     */
    private SetGamePhase(phase: Logic_RoundState): void {
        if (phase !== this.gamePhase) {

            //leave old state
            switch (this.gamePhase) {
                default: break;
                case Logic_RoundState.NOTSTARTED: break;
                case Logic_RoundState.ACCEPTINGBETS: 
                    GLUI.ShowBetUI(false);
                break;
                case Logic_RoundState.CLOSEDFORBETS: break;
                case Logic_RoundState.COMPLETE: 
                    GLUI.ShowWinUI(false);
                break;
            }

            //set new game state
            console.log('Game entering new state: ' + phase);
            this.gamePhase = phase;

            //setup new state
            switch (this.gamePhase) {
                default: break;
                case Logic_RoundState.NOTSTARTED: break;
                case Logic_RoundState.ACCEPTINGBETS: 
                    GLUI.ShowBetUI(true);
                    GLUI.SetPlayerListMode(UIPlayerDisplayMode.BET);
                break;
                case Logic_RoundState.CLOSEDFORBETS: break;
                case Logic_RoundState.COMPLETE: 
                    GLUI.SetPlayerListMode(UIPlayerDisplayMode.WIN);
                    GLUI.ShowWinUI(true);
                break;
            }
        }
    }

}

/**
 * Global Singleton reference to the asset pack
 */
 let MULTITRADER!: MultiTrader;
 export { MULTITRADER };
