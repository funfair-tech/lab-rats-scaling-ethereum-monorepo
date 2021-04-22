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
    labRatsContractAddress: '0x2a810409872AfC346F9B5b26571Fd6eC42EA4849',
    // THIS SHOULD ONLY BE REDEPLOYED IF CODE HAS CHANGED
    // IF YOU WANT TO REDEPLOY IT PUT THIS AS UNDEFINED
    faucetContractAddress: '0xC58D83c8a5EA4d1CC4ceb66d61Fa4Fd8Ea983B12',
  },
];

// Just a standard hardhat-deploy deployment definition file!
const func = async (hre) => {
  const { deployments, getNamedAccounts, getChainId } = hre;
  const { deploy } = deployments;
  const { deployer } = await getNamedAccounts();

  const chainId = await hre.getChainId();
  const deployingNetworkContext = deployingNetworksContext.find(
    (d) => d.chainId === chainId
  );

  const labRatsContractAddress = await deployLabRatsErc667Contract(
    deployingNetworkContext,
    deploy,
    deployer,
    hre
  );

  await deployFaucetContract(
    deployingNetworkContext,
    labRatsContractAddress,
    deploy,
    deployer,
    hre
  );
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

const deployLabRatsErc667Contract = async (
  deployingNetworkContext,
  deploy,
  deployer,
  hre
) => {
  if (
    deployingNetworkContext &&
    deployingNetworkContext.labRatsContractAddress
  ) {
    console.log(
      'Lab rats token is already deployed',
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
  deployingNetworkContext,
  labRatContractAddress,
  deploy,
  deployer,
  hre
) => {
  if (
    deployingNetworkContext &&
    deployingNetworkContext.faucetContractAddress
  ) {
    console.log(
      'Facade is already deployed',
      deployingNetworkContext.faucetContractAddress
    );
    return deployingNetworkContext.faucetContractAddress;
  }
  const faucetResult = await deploy(
    'Faucet',
    buildUpDeployOptions([labRatContractAddress], deployer, hre)
  );

  return faucetResult.address;
};

func.tags = ['deploy-all'];
module.exports = func;
