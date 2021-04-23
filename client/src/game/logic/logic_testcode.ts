/**
 * logic_testcode.ts
 * 
 * test code for logic. This is where the local version of the game is played
 * 
 * can be drived by keys
 */

import { LOGIC_SERVERFEEDQUEUE } from './logic_serverfeedqueue';
import { Logic_BetType, Logic_GameState, Logic_Configuration,Logic_RoundState, Logic_BetWinData, Logic_Bet } from './logic_defines';


export class Logic_TestCode {
    
    static Create(): void {
        LOGIC_TESTCODE = new Logic_TestCode();        
    }

    protected tickCount: number = 0;
    protected roundID: number = 0;
    protected testGameState: Logic_GameState = new Logic_GameState();
    protected configuration: Logic_Configuration = new Logic_Configuration(1000, 10, 5, 100, 30);

    constructor() {
        this.MakeRandomPreviousGamePlay();
        LOGIC_SERVERFEEDQUEUE.SendDummyMessage('CONFIGURATION', {
            configuration:this.configuration
        });
    }

    public Tick(): void {
        //For now it play through sequences

        this.tickCount++;

        if(this.tickCount === 100) {
            //Start the round
            this.NextRoundID();
            this.StartBettingOnRound(this.GetRoundID(),'fakernghash');
        }

        if(this.tickCount === 150) {
            //Stop betting
            this.StopBettingOnRound(this.GetRoundID());
        }

        if(this.tickCount === 200) {
            //End round

            this.EndRound(this.GetRoundID(), Math.floor(Math.random() * 0xffff).toString(16));

            this.tickCount = 0;
        }
    }

    public StartBettingOnRound(_roundID: string, commitHash: string): void {

        let state: Logic_GameState = this.testGameState;

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
                bets: state.bets,
                currentPrizePool: state.currentPrizePool,
                carryOverPrizePool: state.carryOverPrizePool
            });
        }
    }
    
    public StopBettingOnRound(_roundID: string): void {

        let state: Logic_GameState = this.testGameState;

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

        let state: Logic_GameState = this.testGameState;

        //End the round and pay out 

        if(state.roundID === _roundID) {
            if(state.roundState === Logic_RoundState.CLOSEDFORBETS) {
                //Contract would verify hash and permission to close too

                let thisRoundAdjust: number = parseInt(commitRNG, 16) %  ((2 * (this.configuration.stepMaxDeltaRange + 1)) - (this.configuration.stepMaxDeltaRange + 1));
            
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
                    let thisBet: Logic_Bet = state.bets[index];
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
                    let thisBet: Logic_Bet = state.bets[winData.betIndex];
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
        
        
        
        // LOGIC_SERVERFEEDQUEUE.SendDummyMessage('PLACEBET', {
        //     nonce: _nonce
        //     roundID: _roundID,
        //     address: _address,
        //     amount: _amount,
        //     betType: _betType
        // });
    }

    public NextRoundID(): void {
        this.roundID++;
    }

    public GetRoundID(): string {
        return 'TestRoundID_' + this.roundID;
    }

    //Code to handle local game

    protected MakeRandomPreviousGamePlay(): void {
        
        let state: Logic_GameState = this.testGameState;

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