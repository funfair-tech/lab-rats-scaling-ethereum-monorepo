import { TransactionResponse } from '@ethersproject/providers';

export interface IConnectionTest {

  setMessage(newMessage: string): Promise<TransactionResponse>;

  getMessage(): Promise<string>;

  getMessageWithArgs(prefix: string): Promise<string>;

  setMessageSubject(newSubject: string, newMessage: string): void;
}