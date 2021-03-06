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

    private static playerAddress: string = '0x1234567fakeplayer89abcdef';

    private startupFinished: boolean = false;
    private localGame: boolean = false;
    private gamePhase: Logic_RoundState = Logic_RoundState.NOTSTARTED;
    private lastNonce: number = 0;
    private initialStateReceived: boolean = false;

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

        this.localGame = params.isLocalGame;
        Logic.Create(new Logic_Configuration(1000, 10, 5, 100, 30), this.localGame);
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

        if (this.localGame === true) {
            let betResponse = LOGIC.PlaceBetForLocalPlayer(MultiTrader.playerAddress, betType);
            console.log('bet response: ' + betResponse);
        }
        else {
            let betResponse = LOGIC.PlaceBetToServer(betType);
            console.log('bet response: ' + betResponse);
        }
        GRAPH_MANAGER.GraphUILockBetButtons(true);
    }

    public GraphResultFinished(): void {
        GLUI.SetPlayerListMode(UIPlayerDisplayMode.WIN);
        GLUI.ShowWinUI(true);
        LOGIC.UnFreezeBalance();
        GRAPH_MANAGER.HighlightWinningButtons(true);
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

        //advance state if necessary
        if (this.lastNonce !== state.localNonce) {

            //add initial history and values when they exist in the state
            if (this.initialStateReceived === false && state.historicPrices.length > 0) {
                this.initialStateReceived = true;

                //add offset so the graph is at the origin
                GRAPH_MANAGER.OffsetLine(state.historicPrices.length + 1);

                for (let i=state.historicPrices.length-1;i>=0;i--) {
                    GRAPH_MANAGER.AddResult(state.historicPrices[i]);
                }
                GRAPH_MANAGER.AddResult(state.currentPrice, true, true);
            }

            this.lastNonce = state.localNonce;

            //add a new graph result
            if (state.roundState === Logic_RoundState.COMPLETE) {
                GRAPH_MANAGER.AddResult(state.currentPrice, false, true);
                GRAPH_MANAGER.AddWinningBetTypes(state.betWinFlags);
            }

            //update prize pool
            GLUI.SetPrizePool(this.GetRoundedValue(this.localGame ? state.currentPrizePool : state.currentPrizePool / 100000000));

            //update bets and players
            PLAYER_MANAGER.UpdateBets(state.bets, this.localGame ? 1 : 100000000);

            //set the game phase
            this.SetGamePhase(state.roundState);
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
                    //GRAPH_MANAGER.GraphUIShowBetButtons(false);
                break;
                case Logic_RoundState.CLOSEDFORBETS: 
                    GLUI.ShowAwaitingResultUI(false);
                break;
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
                    GRAPH_MANAGER.GraphUIShowBetButtons(true);
                    GRAPH_MANAGER.GraphUILockBetButtons(false);
                    GLUI.SetPlayerListMode(UIPlayerDisplayMode.BET);
                    GRAPH_MANAGER.UpdateGraphUI();
                break;
                case Logic_RoundState.CLOSEDFORBETS: 
                    GRAPH_MANAGER.GraphUILockBetButtons(true);
                    GLUI.ShowAwaitingResultUI(true);
                break;
                case Logic_RoundState.COMPLETE: 
                    
                break;
            }
        }
    }

    public GetRoundedValue(value: number): number {
        return Math.round(value * 100) / 100;
    }

}

/**
 * Global Singleton reference to the asset pack
 */
 let MULTITRADER!: MultiTrader;
 export { MULTITRADER };
