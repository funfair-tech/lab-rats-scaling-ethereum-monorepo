const { getGameManagerContract } = require('./contracts-and-abis');

async function main() {
  const contract = getGameManagerContract();

  const response = await contract.owner();
  console.log('Owner:');
  console.log(response);

  const adminResponse = await contract.admins(
    '0x16485F14e561214E2DFfBffDD5757059E8c74CA3'
  );
  console.log('Admin:');
  console.log(adminResponse);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
