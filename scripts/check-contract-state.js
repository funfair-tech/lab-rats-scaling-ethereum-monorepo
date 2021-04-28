const { getGameManagerContract } = require('./contracts-and-abis');

async function main() {
  const contract = getGameManagerContract();

  const response = await contract.contractState();
  console.log(response);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
