const { getGameManagerContract } = require('./contracts-and-abis');

async function main() {
  const contract = getGameManagerContract();

  const response = await contract.permittedGameDefinitions(
    '0x1804365F80251b3c38185Eb7756B26D069D5A437'
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
