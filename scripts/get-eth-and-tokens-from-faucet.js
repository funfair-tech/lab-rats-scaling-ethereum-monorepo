const { getFaucetContract } = require('./contracts-and-abis');

async function main() {
  const contract = getFaucetContract();
  await contract.setAdmin(l2Wallet.address, true);

  const response = await contract.distributeTokenAndEth(
    '0xBeEB91a350c6721d815e85dBAcF8D41E81819Da3',
    parseEther('0.001').toHexString(),
    '0x01',
    {
      gasLimit: 8000000,
      gasPrice: '0x00',
    }
  );
  console.log(response);
  await response.wait();
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
