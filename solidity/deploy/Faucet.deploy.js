// Just a standard hardhat-deploy deployment definition file!
const func = async (hre) => {
  const { deployments, getNamedAccounts } = hre;
  const { deploy } = deployments;
  const { deployer } = await getNamedAccounts();

  await deploy('LabRats', {
    from: deployer,
    args: [],
    gasPrice: hre.ethers.BigNumber.from('1000000000'),
    gasLimit: 8999999,
    log: true,
  });
};

func.tags = ['LabRats'];
module.exports = func;
