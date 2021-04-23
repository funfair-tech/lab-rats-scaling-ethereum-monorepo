
export interface FaucetResponse { 
  ethAmount: string;
  tokenAmount: string;
  transaction: {
    transactionHash: string,
    network: string,
  }
  message?: string;
}