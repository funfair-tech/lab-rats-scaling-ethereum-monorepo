export interface Bet {
  roundId: string;
  address: string;
  amount: number;
  data: number;
}

export class SafeBet implements Bet {
  constructor(
    public roundId: string,
    public address: string,
    public amount: number,
    public data: any,
    public confirmed: boolean, // Confirned is true if originated from a chain event, false if via the server
  ) {
    if (amount !== 10000000000) {
      throw new Error('Invalid bet amount');
    }

    if (!/^(0x)?[0-9a-f]{40}$/i.test(address)) {
      throw new Error('Invalid address');
    }

    if (!/^(0x)?[0-9a-f]{6}$/i.test(data)) {
      throw new Error('Invalid data');
    }
  }
}