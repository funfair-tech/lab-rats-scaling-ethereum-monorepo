const deployingNetworkContext = {
  // WE NEED A FIXED ADDRESS FOR THE TOKENS
  // ONLY CHANGE THIS IF YOU CHANGED THE LOGIC OF THE CONTRACT
  labRatsContractAddress: '0x11160251d4283A48B7A8808aa0ED8EA5349B56e2',
  // WE NEED A FIXED ADDRESS FOR THE FAUCET
  // ONLY CHANGE THIS IF YOU CHANGED THE LOGIC OF THE CONTRACT
  faucetContractAddress: '0x4697d0CB9E40699237d0f40F3EE211527a5619fF',
};

const func = async (hre) => {
  const { deployments, getNamedAccounts } = hre;
  const { deploy } = deployments;
  const { deployer } = await getNamedAccounts();

  const labRatsContractAddress = await deployLabRatsErc667Contract(
    deploy,
    deployer,
    hre
  );

  await deployFaucetContract(labRatsContractAddress, deploy, deployer, hre);

  await deployMultiplayerGamesManagerContract(
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

const deployLabRatsErc667Contract = async (deploy, deployer, hre) => {
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

const deployMultiplayerGamesManagerContract = async (
  labRatContractAddress,
  deploy,
  deployer,
  hre
) => {
  const multiplayerGamesManagerResult = await deploy(
    'MultiplayerGamesManager',
    buildUpDeployOptions(
      [
        labRatContractAddress,
        [
          '0x16485F14e561214E2DFfBffDD5757059E8c74CA3',
          '0x1F9fcF0A1390C60b88f68e2912eA5f2673413C49',
        ],
      ],
      deployer,
      hre
    )
  );

  console.log('MultiplayerGamesManager ABI:');
  console.log(JSON.stringify(multiplayerGamesManagerResult.abi, null, 2));

  return multiplayerGamesManagerResult.address;
};

func.tags = ['deploy-all'];
module.exports = func;
