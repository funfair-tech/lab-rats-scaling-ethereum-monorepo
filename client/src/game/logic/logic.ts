/**
 * Logic
 * 
 * Class for logic
 */

import { Logic_GameState, Logic_Configuration, Logic_ServerMessage, Logic_BetType, Logic_BetResponse, Logic_RoundState, Logic_Bet } from './logic_defines';
import { LOGIC_SERVERFEEDQUEUE, Logic_ServerFeedQueue } from './logic_serverfeedqueue';
import { LOGIC_TESTCODE, Logic_TestCode } from './logic_testcode';
import { Logic_SeededName } from './logic_seededname';
import { BigNumber } from 'bignumber.js';

export class Logic {

    protected configuration: Logic_Configuration = new Logic_Configuration(1000, 10, 5, 100, 30);
    protected currentState: Logic_GameState = new Logic_GameState();
    protected reportedState: Logic_GameState = JSON.parse(JSON.stringify(this.currentState));
    protected isLocalMode: boolean = false;
    protected localPlayerAddress: string = '0x1234567fakeplayer89abcdef';
    protected static APIPlaycallback: (stake: number, action: number) => void;

    public static SetAPIPlaycallback(callbackFunction: any) {
        Logic.APIPlaycallback = callbackFunction;
    }    

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

    public PlaceBetToServer(betType: number): Logic_BetResponse {
        if(this.reportedState.roundState !== Logic_RoundState.ACCEPTINGBETS) {
            return Logic_BetResponse.NOTINBETTINGPHASE;
        }

        if((betType < 0) || (betType >= Logic_BetType.NUMBETTYPES)) {
            return Logic_BetResponse.INVALIDBET;
        }

        Logic.APIPlaycallback(this.configuration.betAmount * 100000000, betType);

        return Logic_BetResponse.BETSUBMITTED;
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

            //Fix the current prize pool

            this.RecalculateCurrentPrizePool();

            //Determine any winning bet flags

            this.CorrectBetWinFlags();

            //Update the reported state
        
            this.UpdateReportedState();
        }

        //Change to report (debug)

        if((this.reportedState.serverNonce !== stateServerNonceAtStartOfTick) || (this.reportedState.localNonce !== stateLocalNonceAtStartOfTick)) {
            //console.log('snc new reported state: ' + JSON.stringify(this.reportedState));
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
        } else if (message.type === 'TESTSTARTROUND') {
            handled = this.TESTStartRoundFromFeed(message);
        } else if (message.type === 'TESTCLOSEDFORBETS') {
            handled = this.TESTClosedForBetsFromFeed(message);
        } else if (message.type === 'TESTENDROUND') {
            handled = this.TESTEndRoundFromFeed(message);
        } else if (message.type === 'TESTPLACEBET') {
            let betPlaceResponse: Logic_BetResponse = this.TESTPlaceBetFromFeed(message);

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
        
        //Proper handling

        if (message.type === 'LOCALPLAYERADDRESS') {
            //Note the local player address
            this.localPlayerAddress = message.data;    
            handled = true;        
        } else if (message.type === 'NEW_ROUND') {
            handled = this.NewRoundFromFeed(message);
        } else if (message.type === 'NO_MORE_BETS') {
            handled = this.NoMoreBetsFromFeed(message);
        } else if (message.type === 'RESULT') {
            handled = this.ResultFromFeed(message);
        } else if (message.type === 'HISTORY') {
            handled = this.HistoryFromFeed(message);
        } else if (message.type === 'BETS') {
           
           handled = this.BetsFromFeed(message);
        }
        
        if(handled) {
            this.currentState.localNonce++;
        } else {
            console.log('*** UNHANDLED FEED MESSAGE ***\n' + JSON.stringify(message));
        } 
    }

    protected NewRoundFromFeed(message: any): boolean {
        let state: Logic_GameState = this.currentState;
        state.serverNonce++;
        state.roundID = message.data.id;
        state.serverBlock = message.data.block;
        state.roundState = Logic_RoundState.ACCEPTINGBETS;
        state.bets = [];
        return true;
    }

    protected NoMoreBetsFromFeed(message: any): boolean {
        let state: Logic_GameState = this.currentState;
        state.serverNonce++;
        state.roundID = message.data.id;
        state.roundState = Logic_RoundState.CLOSEDFORBETS;
        return true;
    }

    protected ResultFromFeed(message: any): boolean {
        let state: Logic_GameState = this.currentState;
        state.serverNonce++;
        state.roundID = message.data.id;
        state.roundState = Logic_RoundState.COMPLETE;

        state.currentPrice = parseInt('0x' + message.data.result.substr(2,64));
        let movement: number = parseInt('0x' + message.data.result.substr(192,2));
        if(movement & 0x80) {
            movement = - (256 - movement);
        }
        state.lastAdjustment = movement;
        let currentHistoricPrice: number = state.currentPrice;
        for(let index: number = 0; index < 32; index++) {
            let byte: number = parseInt('0x' + message.data.result.substr(192 - (index * 2), 2));
            if(byte & 0x80) {
                byte = -(256 - byte);    
            }
            currentHistoricPrice -= byte;
            state.historicPrices[index] = currentHistoricPrice;                    
        }

        let potWinLoss: BigNumber = new BigNumber(message.data.potWinLoss.hex);        
        state.carryOverPrizePoolAfterResult = parseInt('0x' + message.data.history.substr(130, 64));
        state.carryOverPrizePool = state.carryOverPrizePoolAfterResult - potWinLoss.toNumber();

        //Go through player bets  and rebuild their bet data from the playerAddresses and winnings data

        let playerAddresses = message.data.playerAddresses;
        let winAmounts = message.data.winAmounts;

        if(playerAddresses.length === winAmounts.length) {
            for(let index: number = 0; index < playerAddresses.length; index++) {
                let matchingBetIndex: number = state.bets.findIndex(currentBet => currentBet.address === playerAddresses[index]);
                if(matchingBetIndex !== -1) {
                    state.bets[matchingBetIndex].winnings = parseInt(winAmounts[index]);
                    state.bets[matchingBetIndex].resolved = true;
                }
            }
        }
        return true;
    }

    protected HistoryFromFeed(message: any): boolean {
        let state: Logic_GameState = this.currentState;
        
        state.serverNonce++;
        state.roundState = Logic_RoundState.COMPLETE;

        state.currentPrice = parseInt('0x' + message.data.substr(2,64));
        let currentHistoricPrice = state.currentPrice;
        for(let index: number = 0; index < 32; index++) {
            let byte: number = parseInt('0x' + message.data.substr(128 - (index * 2), 2));
            if(byte & 0x80) {
                byte = -(256 - byte);    
            }
            currentHistoricPrice -= byte;
            state.historicPrices[index] = currentHistoricPrice;
        }
        state.lastAdjustment = state.currentPrice - state.historicPrices[0];        
        state.carryOverPrizePool = parseInt('0x' + message.data.substr(130, 64));
        state.carryOverPrizePoolAfterResult = state.carryOverPrizePool;

        return true;
    }

    protected BetsFromFeed(message: any): boolean {
        
        let state: Logic_GameState = this.currentState;
        let data: any =  message.data;

        state.serverNonce++;

        let newBets: any[] = data;

        newBets.forEach((bet: any) => {
            //Is the bet the correct round?
            //console.log('snc this  bet: ' + JSON.stringify(bet));

            if(state.roundID !== bet.roundId) {
                //Ignore

            } else {

                //Is the bet already here?
                    
                let matchingBetIndex: number = state.bets.findIndex(currentBet => currentBet.address === bet.address);
                if(matchingBetIndex !== -1) {
                    //Just adjust the confirmed state

                    state.bets[matchingBetIndex].confirmed = true;
                } else {

                    //Add the bet to the current state
                    state.bets.push(new Logic_Bet(bet.address, Logic_SeededName.GetNameFromString(bet.address), parseInt(bet.amount), parseInt(bet.data), (bet.address === this.localPlayerAddress)));
                }
            }            
        });

        //Handled

        return true;
    }

    protected RecalculateCurrentPrizePool(): void {
        let state: Logic_GameState = this.currentState;

        //Work out the current prize pool

        if(state.roundState === Logic_RoundState.COMPLETE) {
            //Special case for this
        } else {
            state.currentPrizePool = state.carryOverPrizePoolAfterResult;
            state.bets.forEach((bet) => {
                state.currentPrizePool += bet.amount;
            });
        }
    }

    protected CorrectBetWinFlags(): void {
        let state: Logic_GameState = this.currentState;

        //Clear out bet win flags

        state.betWinFlags = [false, false, false, false, false, false];
        
        //Set only if state is complete

        if(state.roundState === Logic_RoundState.COMPLETE) {

            //Determine winners

            if(state.lastAdjustment > 0) {
                state.betWinFlags[Logic_BetType.HIGHER] = true;
                if(state.lastAdjustment > 5) {
                    state.betWinFlags[Logic_BetType.LARGEHIGHER] = true;                    
                } else {
                    state.betWinFlags[Logic_BetType.SMALLHIGHER] = true;
                }
            } else if (state.lastAdjustment < 0) {
                state.betWinFlags[Logic_BetType.LOWER] = true;
                if(state.lastAdjustment < -5) {
                    state.betWinFlags[Logic_BetType.LARGELOWER] = true;                    
                } else {
                    state.betWinFlags[Logic_BetType.SMALLLOWER] = true;
                }
            }
        }
    }

    protected TESTStartRoundFromFeed(message: any): boolean {
    
        if(message.type !== 'TESTSTARTROUND') {
            return false;
        }

        let state: Logic_GameState = this.currentState;

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

    protected TESTClosedForBetsFromFeed(message: any): boolean {
    
        if(message.type !== 'TESTCLOSEDFORBETS') {
            return false;
        }

        let state: Logic_GameState = this.currentState;

        state.serverNonce = message.data.nonce;
        state.roundID = message.data.roundID;
        state.roundState = message.data.roundState;
        return true;
    }

    protected TESTEndRoundFromFeed(message: any): boolean {
        //Ensure it is correct message type

        if(message.type !== 'TESTENDROUND') {
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
        state.carryOverPrizePool = message.data.carryOverPrizePool - message.data.potWinLoss;
        state.carryOverPrizePoolAfterResult = message.data.carryOverPrizePool;
        return true;
    }

    protected TESTPlaceBetFromFeed(message: any): Logic_BetResponse {
        //Ensure it is correct message type

        if(message.type !== 'TESTPLACEBET') {
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

