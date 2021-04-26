export interface IConnectionTest {

  setMessage(newMessage: string): void;

  getMessage(): Promise<string>;

  getMessageWithArgs(prefix: string): Promise<string>;

  setMessageSubject(newSubject: string, newMessage: string): void;
}