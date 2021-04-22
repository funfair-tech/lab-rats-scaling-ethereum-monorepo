const deployingNetworksContext = [
  {
    name: 'KOVAN',
    chainId: '42',
    // WE NEED A FIXED ADDRESS FOR THE TOKENS
    // ONCE DEPLOYED PUT IT IN HERE AND THE
    // DEPLOYMENT SCRIPT WILL HANDLE IT FOR YOU
    labRatsContractAddress: '0x64f5361a555A43776f71A06C58dD7bCD7E184983',
  },
  {
    name: 'OPTIMISMKOVAN',
    chainId: '69',
    // WE NEED A FIXED ADDRESS FOR THE TOKENS
    // ONCE DEPLOYED PUT IT IN HERE AND THE
    // DEPLOYMENT SCRIPT WILL HANDLE IT FOR YOU
    labRatsContractAddress: undefined,
  },
];

// Just a standard hardhat-deploy deployment definition file!
const func = async (hre) => {
  const { deployments, getNamedAccounts, getChainId } = hre;
  const { deploy } = deployments;
  const { deployer } = await getNamedAccounts();

  const labRatsContractAddress = await deployLabRatsErc667Contract(
    deploy,
    deployer,
    hre
  );

  await deployFaucetContract(labRatsContractAddress, deploy, deployer, hre);
};

const buildUpDeployOptions = (args, deployer, hre) => {
  return {
    from: deployer,
    args,
    gasPrice: hre.ethers.BigNumber.from('0'),
    gasLimit: 8999999,
    log: true,
  };
};

const deployLabRatsErc667Contract = async (deploy, deployer, hre) => {
  const chainId = await hre.getChainId();
  console.log('hey', chainId);
  const deployingNetworkContext = deployingNetworksContext.find(
    (d) => d.chainId === chainId
  );

  if (
    deployingNetworkContext &&
    deployingNetworkContext.labRatsContractAddress
  ) {
    console.log(
      `Lab rats token is already deployed`,
      deployingNetworkContext.labRatsContractAddress
    );
    return deployingNetworkContext.labRatsContractAddress;
  }

  const labRatsResults = await deploy(
    'LabRats',
    buildUpDeployOptions([], deployer, hre)
  );

  return labRatsResults.address;
};

const deployFaucetContract = async (
  labRatContractAddress,
  deploy,
  deployer,
  hre
) => {
  const faucetResult = await deploy(
    'Faucet',
    buildUpDeployOptions([labRatContractAddress], deployer, hre)
  );

  return faucetResult.address;
};

func.tags = ['deploy-all'];
module.exports = func;
