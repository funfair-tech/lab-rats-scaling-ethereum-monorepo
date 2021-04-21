export interface BlockHeader {
  networkId: number;
  blockNumber: number;
  blockHash: string;
  bloomFilter: string;
  timestamp: number;
}
