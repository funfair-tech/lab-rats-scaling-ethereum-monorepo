/**
 * Logic
 * 
 * Class for logic
 */

import { Logic_GameState, Logic_Configuration, Logic_ServerMessage, Logic_BetType, Logic_BetResponse, Logic_RoundState, Logic_Bet } from './logic_defines';
import { LOGIC_SERVERFEEDQUEUE, Logic_ServerFeedQueue } from './logic_serverfeedqueue';
import { LOGIC_TESTCODE, Logic_TestCode } from './logic_testcode';
import { Logic_SeededName } from './logic_seededname';

export class Logic {

    protected configuration: Logic_Configuration = new Logic_Configuration(1000, 10, 5, 100, 30);
    protected currentState: Logic_GameState = new Logic_GameState();
    protected reportedState: Logic_GameState = JSON.parse(JSON.stringify(this.currentState));
    protected isLocalMode: boolean = false;
    protected localPlayerAddress: string = 'tobedone';

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

    /**
     * CALL FOR THE GAME TO MAKE TO THE LOGIC
     */

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

        let stateServerNonceAtStartOfTick: number = this.reportedState.serverNonce;
        let stateLocalNonceAtStartOfTick: number = this.reportedState.localNonce;

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

        if((this.reportedState.serverNonce !== stateServerNonceAtStartOfTick) || (this.reportedState.localNonce !== stateLocalNonceAtStartOfTick)) {
            console.log('snc new reported state: ' + JSON.stringify(this.reportedState));
        }
        
    }

    /**
     * INTERNAL CALLS
     */
    //Handle messages

    protected FeedFromWebSocket(message: Logic_ServerMessage): void { 

        let handled: boolean = false;

        //Handle the message

        if(message.type === 'CONFIGURATION') {
            //Set to this configuration

            this.configuration = message.data.configuration;     
            handled = true;
        } else if (message.type === 'STARTROUND') {
            handled = this.StartRoundFromFeed(message);
        } else if (message.type === 'CLOSEDFORBETS') {
            handled = this.ClosedForBetsFromFeed(message);
        } else if (message.type === 'ENDROUND') {
            handled = this.EndRoundFromFeed(message);
        } else if (message.type === 'PLACEBET') {
            let betPlaceResponse: Logic_BetResponse = this.PlaceBetFromFeed(message);

            if(betPlaceResponse === Logic_BetResponse.NONE) {
                handled = false;
            } else if(betPlaceResponse !== Logic_BetResponse.BETSUBMITTED) {
                //Need to warn UI of bet being refused
                console.log('need to warn UI of bet refusal for message: ' + JSON.stringify(message));
                handled = true;
            } else {
                handled = true;
            }
        } 
        
        if(handled) {
            this.currentState.localNonce++;
        } else {
            console.log('*** UNHANDLED FEED MESSAGE ***\n' + JSON.stringify(message));
        } 
    }

    protected StartRoundFromFeed(message: any): boolean {
    
        if(message.type !== 'STARTROUND') {
            return false;
        }

        let state: Logic_GameState = this.currentState;

        state.serverNonce = message.data.nonce;
        state.roundID = message.data.roundID;
        state.roundState = message.data.roundState;
        state.currentPrice = message.data.currentPrice;
        state.historicPrices = message.data.historicPrices;
        state.lastAdjustment = message.data.lastAdjustment;
        state.bets = [];
        state.currentPrizePool = message.data.currentPrizePool;
        state.carryOverPrizePool = message.data.carryOverPrizePool;

        return true;
    }

    protected ClosedForBetsFromFeed(message: any): boolean {
    
        if(message.type !== 'CLOSEDFORBETS') {
            return false;
        }

        let state: Logic_GameState = this.currentState;

        state.serverNonce = message.data.nonce;
        state.roundID = message.data.roundID;
        state.roundState = message.data.roundState;
        return true;
    }

    protected EndRoundFromFeed(message: any): boolean {
        //Ensure it is correct message type

        if(message.type !== 'ENDROUND') {
            return false;
        }

        let state: Logic_GameState = this.currentState;

        state.serverNonce = message.data.nonce;
        state.roundID = message.data.roundID;
        state.roundState = message.data.roundState;
        state.currentPrice = message.data.currentPrice;
        state.historicPrices = message.data.historicPrices;
        state.lastAdjustment = message.data.lastAdjustment;

        state.bets = [];
        
        message.data.bets.forEach((bet: any) => {
            state.bets.push(this.MakeLogicBetFromServerBet(bet));
        });

        //Need to run though bets and add/apply correctly with additional data such as friendly name and winnings

        state.currentPrizePool = message.data.currentPrizePool;
        state.carryOverPrizePool = message.data.carryOverPrizePool;
        return true;
    }

    protected PlaceBetFromFeed(message: any): Logic_BetResponse {
        //Ensure it is correct message type

        if(message.type !== 'PLACEBET') {
            return Logic_BetResponse.NONE;
        } 
        
        if(message.data.response !== Logic_BetResponse.BETSUBMITTED) {
            return message.data.response;
        }

        let state: Logic_GameState = this.currentState;

        //Check the bet valid for placement, and still for the current round

        if(state.roundID !== message.data.roundID) {
            return Logic_BetResponse.INVALIDROUNDID;
        }

        //Place the bet

        state.bets.push(this.MakeLogicBetFromServerBet(message.data));

        //Add the bet value to the current prize pool

        state.currentPrizePool += message.data.amount;
        return Logic_BetResponse.BETSUBMITTED;
    }

    protected MakeLogicBetFromServerBet(serverBet: any): Logic_Bet {
        
        let newBet: Logic_Bet = new Logic_Bet(
            serverBet.address,
            Logic_SeededName.GetNameFromString(serverBet.address),
            serverBet.amount,
            serverBet.betType,
            (serverBet.address === this.localPlayerAddress)
        );

        if(serverBet.winnings !== undefined) {
            newBet.winnings = serverBet.winnings;
        }
            
        if(serverBet.resolved !== undefined) {
            newBet.resolved = serverBet.resolved;
        }
        
        return newBet;
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

