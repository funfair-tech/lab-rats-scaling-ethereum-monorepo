/**
 * Logic
 * 
 * Class for logic
 */

import { Logic_GameState, Logic_Configuration, Logic_ServerMessage, Logic_BetType, Logic_BetResponse, Logic_RoundState } from './logic_defines';
import { LOGIC_SERVERFEEDQUEUE, Logic_ServerFeedQueue } from './logic_serverfeedqueue';
import { LOGIC_TESTCODE, Logic_TestCode } from './logic_testcode';

export class Logic {

    protected configuration: Logic_Configuration = new Logic_Configuration(1000, 10, 5, 100, 30);
    protected currentState: Logic_GameState = new Logic_GameState();
    protected reportedState: Logic_GameState = JSON.parse(JSON.stringify(this.currentState));
    protected isLocalMode: boolean = false;

    static Create(config: Logic_Configuration, localMode: boolean): void {
        Logic_ServerFeedQueue.Create();
        Logic_TestCode.Create();
        LOGIC = new Logic(config, localMode);        
    }

    constructor(configuration: Logic_Configuration, localMode: boolean) {
        
        //Set up mode

        this.isLocalMode = localMode;
        this.configuration = configuration;
    }

    public CheckLocalMode(): boolean {
        return this.isLocalMode;
    }

    public GetCurrentState() : Logic_GameState {
        return this.reportedState;
    }

    public PlaceBetForLocalPlayer(playerAddress: string, betType: Logic_BetType): Logic_BetResponse {

        //Does the local state show a suitable stage to place bet?

        if(this.reportedState.roundState !== Logic_RoundState.ACCEPTINGBETS) {
            return Logic_BetResponse.NOTINBETTINGPHASE;
        }

        if((betType < 0) || (betType >= Logic_BetType.NUMBETTYPES)) {
            return Logic_BetResponse.INVALIDBET;
        }

        //Assume all is ok, and send request to server (for now to test logic)

        LOGIC_TESTCODE.PlaceBet(this.reportedState.roundID, playerAddress, this.configuration.betAmount, betType);
        return Logic_BetResponse.BETSUBMITTED;
    }

    public Tick(): void {        

        let stateNonceAtStartOfTick: number = this.reportedState.nonce;

        LOGIC_TESTCODE.Tick();
        
        //Check and response to the message queue

        if(LOGIC_SERVERFEEDQUEUE.CountServerMessages() !== 0) {
            let message: Logic_ServerMessage | null;

            while((message = LOGIC_SERVERFEEDQUEUE.GetNextServerMessage()) !== null) {

                //Handle the feed

                this.FeedFromWebSocket(message);
            }

            //Update the reported state
        
            this.UpdateReportedState();
        }

        //Change to report (debug)

        if(this.reportedState.nonce !== stateNonceAtStartOfTick) {
            console.log('snc new reported state: ' + JSON.stringify(this.reportedState));
        }
        
    }

    //Handle messages

    public FeedFromWebSocket(message: Logic_ServerMessage): void { 

        //Handle the message

        if(message.type === 'CONFIGURATION') {
            //Set to this configuration

            this.configuration = message.data.configuration;     

        } else if (message.type === 'STARTROUND') {
            //Update the game with the new data

            this.StartRoundFromFeed(message);
        } else if (message.type === 'CLOSEDFORBETS') {
            //Update the game with the new data

            this.ClosedForBetsFromFeed(message);
        } else if (message.type === 'ENDROUND') {
            //Update the game with the new data

            this.EndRoundFromFeed(message);
        } else {
            console.log('*** UNHANDLED FEED MESSAGE ***');
        }
    }

    protected StartRoundFromFeed(message: any): void {
    
        if(message.type !== 'STARTROUND') {
            return;
        }

        let state: Logic_GameState = this.currentState;

        state.nonce = message.data.nonce;
        state.roundID = message.data.roundID;
        state.roundState = message.data.roundState;
        state.currentPrice = message.data.currentPrice;
        state.historicPrices = message.data.historicPrices;
        state.lastAdjustment = message.data.lastAdjustment;
        state.bets = message.data.bets;
        state.currentPrizePool = message.data.currentPrizePool;
        state.carryOverPrizePool = message.data.carryOverPrizePool;
    }

    protected ClosedForBetsFromFeed(message: any): void {
    
        if(message.type !== 'CLOSEDFORBETS') {
            return;
        }

        let state: Logic_GameState = this.currentState;

        state.nonce = message.data.nonce;
        state.roundID = message.data.roundID;
        state.roundState = message.data.roundState;
    }

    protected EndRoundFromFeed(message: any): void {
        //Ensure it is correct message type

        if(message.type !== 'ENDROUND') {
            return;
        }

        let state: Logic_GameState = this.currentState;

        state.nonce = message.data.nonce;
        state.roundID = message.data.roundID;
        state.roundState = message.data.roundState;
        state.currentPrice = message.data.currentPrice;
        state.historicPrices = message.data.historicPrices;
        state.lastAdjustment = message.data.lastAdjustment;
        state.bets = message.data.bets;
        state.currentPrizePool = message.data.currentPrizePool;
        state.carryOverPrizePool = message.data.carryOverPrizePool;
    }

    protected UpdateReportedState(): void {
  
        //Report it

        this.reportedState = JSON.parse(JSON.stringify(this.currentState));
    }

    
}
/**
 * Global Singleton reference to the logic
 */

 let LOGIC!: Logic;
 export { LOGIC };

