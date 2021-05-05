
/**
 * logic_defines.ts
 * 
 * defines for the game logic
 */

export class Logic_ServerMessage {
    public type: string = '';
    public data: any = {};

    constructor(_type: string) {
        this.type = _type;
    }
}

export class Logic_Configuration {
    public baseValue: number = 1000;
    public stepMaxDeltaRange: number = 10;
    public lowerRange: number = 5;    
    public betAmount: number = 100;
    public historySize: number = 30;

    constructor(_baseValue: number, _stepMaxDeltaRange: number, _lowerRange: number, _betAmount: number, _historySize: number) {
        this.baseValue = _baseValue;
        this.stepMaxDeltaRange = _stepMaxDeltaRange;
        this.lowerRange = _lowerRange;
        this.betAmount = _betAmount;
        this.historySize = _historySize;
    }
}

export enum Logic_RoundState {
    NOTSTARTED = 0,
    ACCEPTINGBETS = 1,
    CLOSEDFORBETS = 2,
    COMPLETE = 3
};

export enum Logic_BetType {
    NONE = -1,
    HIGHER = 0,
    LOWER,
    SMALLHIGHER,
    LARGEHIGHER,
    SMALLLOWER,
    LARGELOWER,
    NUMBETTYPES
}

export enum Logic_BetResponse {
    NONE = -1,
    BETSUBMITTED = 0,
    BETALREADYPLACED,
    NOTINBETTINGPHASE,
    INVALIDROUNDID,
    INVALIDAMOUNT, 
    INVALIDBET
}
export class Logic_Bet {
    public address: string = '';
    public name: string = '';
    public amount: number = 0;
    public betType: Logic_BetType = Logic_BetType.NONE;
    public confirmed: boolean = false;
    public winnings: number = 0;
    public resolved: boolean = false;
    public isLocalPlayer: boolean = false;

    constructor(_address: string, _name: string, _amount: number, _betType: Logic_BetType, _isLocalPlayer: boolean) {
        this.address = _address;
        this.name = _name;
        this.amount = _amount;
        this.betType = _betType;
        this.isLocalPlayer = _isLocalPlayer;
    }

    public Resolve(_winnings: number) {
        this.winnings = _winnings;
        this.resolved = true;
    }    
}

export class Logic_BetWinData {
    public betIndex: number = -1;
    public prizeAllocation: number = 0;

    constructor(_betIndex: number, _prizeAllocation: number) {
        this.betIndex = _betIndex;
        this.prizeAllocation = _prizeAllocation;
    }
}

export class Logic_GameState {
    public roundState: Logic_RoundState = Logic_RoundState.NOTSTARTED;
    public roundID: string = '';
    public currentPrice: number = 0;   
    public historicPrices: number[] = [];    
    public lastAdjustment: number = 0;
    public bets: Logic_Bet[] = [];
    public carryOverPrizePool: number = 0;
    public carryOverPrizePoolAfterResult: number = 0;
    public currentPrizePool: number = 0;

    //Reporting control

    public serverBlock: string = '0x0';
    public serverNonce: number = 0;
    public localNonce: number = 0;
}



