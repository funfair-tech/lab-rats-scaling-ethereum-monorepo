/**
 * logic_testcode.ts
 * 
 * test code for logic. This is where the local version of the game is played
 * 
 * can be drived by keys
 */

import { LOGIC_SERVERFEEDQUEUE } from './logic_serverfeedqueue';
import { Logic_BetType, Logic_Configuration,Logic_RoundState, Logic_BetWinData, Logic_BetResponse } from './logic_defines';
import { LOGIC } from './logic';

export class TestServer_Bet {
    public address: string = '';
    public amount: number = 0;
    public betType: Logic_BetType = Logic_BetType.NONE;
    public winnings: number = 0;
    public resolved: boolean = false;

    constructor(_address: string, _amount: number, _betType: Logic_BetType) {
        this.address = _address;
        this.amount = _amount;
        this.betType = _betType;
    }

    public Resolve(_winnings: number) {
        this.winnings = _winnings;
        this.resolved = true;
    }    
}
export class TestServer_GameState {
    public roundState: Logic_RoundState = Logic_RoundState.NOTSTARTED;
    public roundID: string = '';
    public currentPrice: number = 0;   
    public historicPrices: number[] = [];    
    public lastAdjustment: number = 0;
    public bets: TestServer_Bet[] = [];
    public carryOverPrizePool: number = 0;
    public currentPrizePool: number = 0;

    //Reporting control

    public nonce: number = 0;
}

export class Logic_TestCode {
    
    static Create(): void {
        LOGIC_TESTCODE = new Logic_TestCode();        
    }

    protected tickCount: number = 0;
    protected roundID: number = 0;
    protected testGameState: TestServer_GameState = new TestServer_GameState();
    protected configuration: Logic_Configuration = new Logic_Configuration(1000, 10, 5, 100, 30);

    constructor() {
        this.MakeRandomPreviousGamePlay();
        LOGIC_SERVERFEEDQUEUE.SendDummyMessage('CONFIGURATION', {
            configuration:this.configuration
        });
    }

    public KeyPressed(keyCode: number): void {
        if(keyCode === 48) { //0
            //Advance state

            if((this.testGameState.roundState === Logic_RoundState.NOTSTARTED) || (this.testGameState.roundState === Logic_RoundState.COMPLETE)) {
                this.NextRoundID();
                this.StartBettingOnRound(this.GetRoundID(),'fakernghash');
            } else if(this.testGameState.roundState === Logic_RoundState.ACCEPTINGBETS) {
                this.StopBettingOnRound(this.GetRoundID());
            } else if(this.testGameState.roundState === Logic_RoundState.CLOSEDFORBETS) {
                this.EndRound(this.GetRoundID(), Math.floor(Math.random() * 0xffff).toString(16));
            } else {
                console.log('Logic_TestCode.KeyPressed advance state fails due to state');
            }

            return;
        }

        if((keyCode >= 49) && (keyCode <=54))  { //1 - 6
            
            //Call to place a bet as a local player

            let response: Logic_BetResponse = LOGIC.PlaceBetForLocalPlayer('0x1234567fakeplayer89abcdef', Logic_BetType.HIGHER + (keyCode - 49));
            if(response === Logic_BetResponse.BETSUBMITTED) {
                console.log('Logic_TestCode.Keypressed local player bet type ' + (keyCode - 49) + ' successfully submitted');
            } else {
                console.log('Logic_TestCode.Keypressed local player bet type ' + (keyCode - 49) + ' failed with Logic_BetResponse value ' + response);
            }
    
            return;
        }

    }

    public Tick(): void {
        //For now it play through sequences

        this.tickCount++;        
    }

    public StartBettingOnRound(_roundID: string, commitHash: string): void {

        let state: TestServer_GameState = this.testGameState;

        //If the game is in a suitable state, start betting

        if(state.roundState === Logic_RoundState.COMPLETE) {
            //Start the round

            state.nonce ++;
            state.roundID = _roundID;
            state.bets = [];
            state.currentPrizePool = state.carryOverPrizePool;
            state.roundState = Logic_RoundState.ACCEPTINGBETS;

            LOGIC_SERVERFEEDQUEUE.SendDummyMessage('STARTROUND', {                
                nonce: state.nonce,
                roundID: state.roundID,
                roundState: state.roundState,
                currentPrice: state.currentPrice,
                historicPrices: state.historicPrices,
                lastAdjustment: state.lastAdjustment,
                currentPrizePool: state.currentPrizePool,
                carryOverPrizePool: state.carryOverPrizePool
            });
        }
    }
    
    public StopBettingOnRound(_roundID: string): void {

        let state: TestServer_GameState = this.testGameState;

        //Stop accepting bets

        if(state.roundID === _roundID) {
            if(state.roundState === Logic_RoundState.ACCEPTINGBETS) {
                //Close for bets

                state.nonce++;
                state.roundState = Logic_RoundState.CLOSEDFORBETS;

                LOGIC_SERVERFEEDQUEUE.SendDummyMessage('CLOSEDFORBETS', {
                    nonce: state.nonce,
                    roundID: state.roundID,
                    roundState: state.roundState
                })
            }
        }
    }

    public EndRound(_roundID: string, commitRNG: string): void {

        let state: TestServer_GameState = this.testGameState;

        //End the round and pay out 

        if(state.roundID === _roundID) {
            if(state.roundState === Logic_RoundState.CLOSEDFORBETS) {
                //Contract would verify hash and permission to close too

                let thisRoundAdjust: number = (parseInt(commitRNG, 16) %  ((2 * (this.configuration.stepMaxDeltaRange + 1)))) - (this.configuration.stepMaxDeltaRange + 1);
            
                //End the game round, work out winnings and remaining pool and send it to the client

                state.nonce++;
                state.roundState = Logic_RoundState.COMPLETE;

                //Adjust the history and price

                state.historicPrices.unshift(state.currentPrice);
                if(state.historicPrices.length > this.configuration.historySize) {
                    state.historicPrices.splice(this.configuration.historySize);
                }

                state.lastAdjustment = thisRoundAdjust;
                state.currentPrice += thisRoundAdjust;                

                //Count (and flag) the winning bets, and recalculate the total prize pool from the current bets and retained prize pool

                let winningBetData: Logic_BetWinData[] = [];
                let prizeDistribution: number = 0;
                state.currentPrizePool = state.carryOverPrizePool;

                for(let index: number = 0; index < state.bets.length; index++) {
                    let thisBet: TestServer_Bet = state.bets[index];
                    let thisBetPrizeAllocation: number = 0;

                    state.currentPrizePool += thisBet.amount;

                    if(thisRoundAdjust > 0) {
                        //Look for highers
                    
                        if(thisBet.betType === Logic_BetType.HIGHER) {
                            thisBetPrizeAllocation = 1; //1 prize slice for a HIGHER                    
                        } else {

                            if(thisRoundAdjust <= this.configuration.lowerRange) {
                                if(thisBet.betType === Logic_BetType.SMALLHIGHER) {
                                    thisBetPrizeAllocation = 2; //2 prize slices for a SMALLHIGHER
                                }
                            } else {
                                if(thisBet.betType === Logic_BetType.LARGEHIGHER) {
                                    thisBetPrizeAllocation = 2; //2 prize slices for a LARGEHIGHER
                                }
                            }
                        }
                    } else if(thisRoundAdjust < 0) {
                        //Look for lowers

                        if(thisBet.betType === Logic_BetType.LOWER) {
                            thisBetPrizeAllocation = 1; //1 prize slice for a LOWER
                        } else {

                        if((0 - thisRoundAdjust) <= this.configuration.lowerRange) {
                            if(thisBet.betType === Logic_BetType.SMALLLOWER) {
                                thisBetPrizeAllocation = 2; //2 prize slices for a SMALLLOWER
                            }
                        } else {
                            if(thisBet.betType === Logic_BetType.LARGELOWER) {
                                thisBetPrizeAllocation = 2; //2 prize slices for a LARGELOWER
                            }
                        }
                    }
                    }

                    //Did it win anything?

                    if(thisBetPrizeAllocation !== 0) {
                        winningBetData.push(new Logic_BetWinData(index, thisBetPrizeAllocation));
                        prizeDistribution += thisBetPrizeAllocation;
                    } else {
                        thisBet.Resolve(0);
                    }
                }

                //Payout any bets

                let prizePerPlayer: number = 0;

                if(prizeDistribution !== 0) {
                    //Work out the prize pool to allocate

                    prizePerPlayer = Math.floor(state.currentPrizePool / prizeDistribution);            
                }

                state.carryOverPrizePool = state.currentPrizePool;

                //Payout reducing the carry over prize pool

                winningBetData.forEach((winData) => {
                    let thisBet: TestServer_Bet = state.bets[winData.betIndex];
                    let thisPrize: number = prizePerPlayer * winData.prizeAllocation;
                    thisBet.Resolve(thisPrize);
                    state.carryOverPrizePool -= thisPrize;
                });

                //Report 
                LOGIC_SERVERFEEDQUEUE.SendDummyMessage('ENDROUND', {
                    nonce: state.nonce,
                    roundID: state.roundID,
                    roundState: state.roundState,
                    currentPrice: state.currentPrice,
                    historicPrices: state.historicPrices,
                    lastAdjustment: state.lastAdjustment,
                    bets: state.bets,
                    currentPrizePool: state.currentPrizePool,
                    carryOverPrizePool: state.carryOverPrizePool
                });
            }
        }
    }
    
    public PlaceBet(_roundID: string, _address: string, _amount: number, _betType: Logic_BetType): void {
        
        let response: Logic_BetResponse;
        let state: TestServer_GameState = this.testGameState;

        //Prep the message describing the bet
        
        let messageData: any = {
            nonce: state.nonce,
            roundID: _roundID,
            address: _address,
            amount: _amount,
            betType: _betType,
            response: Logic_BetResponse.NONE
        };

        //Act like the server and see if this bet should be placed

        if(this.testGameState.roundID !== _roundID) {
            response = Logic_BetResponse.INVALIDROUNDID;
        } else if(this.testGameState.roundState !== Logic_RoundState.ACCEPTINGBETS) {
            response = Logic_BetResponse.NOTINBETTINGPHASE;
        } else if(this.configuration.betAmount !== _amount) {
            response = Logic_BetResponse.INVALIDAMOUNT;
        } else if((_betType < 0) || (_betType >= Logic_BetType.NUMBETTYPES)) {
            response = Logic_BetResponse.INVALIDBET;
        } else {
            //Try to place the bet
            
            let matchingBetIndex: number = this.testGameState.bets.findIndex(currentBet => currentBet.address === _address);
            if(matchingBetIndex !== -1) {
                response = Logic_BetResponse.BETALREADYPLACED;
            } else {
                //Add the bet to the current state
                
                state.nonce++;
                state.bets.push(new TestServer_Bet(_address,  _amount, _betType));
                response = Logic_BetResponse.BETSUBMITTED;
            }
        }
        
        //Send the message

        messageData.nonce = state.nonce;
        messageData.response = response;
        LOGIC_SERVERFEEDQUEUE.SendDummyMessage('PLACEBET', messageData);
    }

    public NextRoundID(): void {
        this.roundID++;
    }

    public GetRoundID(): string {
        return 'TestRoundID_' + this.roundID;
    }

    //Code to handle local game

    protected MakeRandomPreviousGamePlay(): void {
        
        let state: TestServer_GameState = this.testGameState;

        //Set up the basic gamestate

        state.roundState = Logic_RoundState.NOTSTARTED;
        state.bets = [];
        state.currentPrice = this.configuration.baseValue;
        state.historicPrices = [];
        state.roundID = '';
        state.carryOverPrizePool = 0;
        state.currentPrizePool = 0;

        for(let i = 0; i < this.configuration.historySize; i++) {
            this.NextRoundID();
            state.roundID = this.GetRoundID();
            let adjust: number = Math.floor(Math.random() * (2 * (this.configuration.stepMaxDeltaRange + 1))) - (this.configuration.stepMaxDeltaRange + 1);
            state.historicPrices.push(state.currentPrice);
            state.currentPrice += adjust;            
        }

        state.roundState = Logic_RoundState.COMPLETE;
    }
}

let LOGIC_TESTCODE!: Logic_TestCode;
export { LOGIC_TESTCODE };