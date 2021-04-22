/**
 * logic_dummyfeed.ts
 * 
 * Provides simulated feeds for testing
 */

import {  Logic_ServerMessage } from './logic_defines';

export class Logic_DummyFeed {

    protected serverMessageQueue: Logic_ServerMessage[] = [];
    protected fakeRoundID: number = 1;
    
    static Create(): void {
        LOGIC_DUMMYFEED = new Logic_DummyFeed();        
    }

    public CountServerMessages(): number {
        return this.serverMessageQueue.length;
    }

    public SendDummyMessage(type: string, data: any): void {
        let message: Logic_ServerMessage = new Logic_ServerMessage(type);
        message.data = data;
        this.serverMessageQueue.push(message);
    }

    public GetRoundID(): string {
        return 'DUMMYROUND_' + this.fakeRoundID;
    }

    public NextRoundID(): void {
        this.fakeRoundID++;
    }

    public GetNextServerMessage(): Logic_ServerMessage | null {
        if(this.serverMessageQueue.length === 0) {
            return null;
        }

        let message: Logic_ServerMessage = this.serverMessageQueue.splice(0, 1)[0];
        return message;
    }

}


let LOGIC_DUMMYFEED!: Logic_DummyFeed;
export { LOGIC_DUMMYFEED };
