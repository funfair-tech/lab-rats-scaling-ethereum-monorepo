/**
 * logic_serverfeedqueue.ts
 * 
 * Provides simulated feeds for testing
 */

import {  Logic_ServerMessage } from './logic_defines';

export class Logic_ServerFeedQueue {

    protected serverMessageQueue: Logic_ServerMessage[] = [];
    
    static Create(): void {
        LOGIC_SERVERFEEDQUEUE = new Logic_ServerFeedQueue();        
    }

    public CountServerMessages(): number {
        return this.serverMessageQueue.length;
    }

    public SendDummyMessage(type: string, data: any): void {
        let message: Logic_ServerMessage = new Logic_ServerMessage(type);
        message.data = data;
        this.serverMessageQueue.push(message);
    }

    public GetNextServerMessage(): Logic_ServerMessage | null {
        if(this.serverMessageQueue.length === 0) {
            return null;
        }

        let message: Logic_ServerMessage = this.serverMessageQueue.splice(0, 1)[0];
        return message;
    }

}

let LOGIC_SERVERFEEDQUEUE!: Logic_ServerFeedQueue;
export { LOGIC_SERVERFEEDQUEUE };
