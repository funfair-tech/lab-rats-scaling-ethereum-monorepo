const { getGameManagerContract } = require('./contracts-and-abis');

async function main() {
  const contract = getGameManagerContract();

  const response = await contract.setContractState(
    0x0000000000000000000000000000000000000000000000000000000000000001,
    {
      gasLimit: 8000000,
      gasPrice: '0x00',
    }
  );
  console.log('Permitted:');
  console.log(response);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
