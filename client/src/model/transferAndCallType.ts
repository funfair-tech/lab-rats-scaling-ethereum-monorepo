
export const TransferAndCallType = {
  name: 'TokenTransferData',
  indexed: false,
  type: 'tuple',
  components: [
    {
      name: 'roundID',
      indexed: false,
      type: 'bytes32',
    },
    {
      name: 'bets',
      indexed: false,
      type: 'tuple[]',
      components: [
        {
          name: 'playerAddress',
          indexed: false,
          type: 'address',
        },
        {
          name: 'betAmount',
          indexed: false,
          type: 'uint256',
        },
        {
          name: 'betData',
          indexed: false,
          type: 'bytes',
        }
      ]
    }
  ]
}
