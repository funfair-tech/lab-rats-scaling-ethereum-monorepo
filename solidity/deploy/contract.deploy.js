// Just a standard hardhat-deploy deployment definition file!
const func = async (hre) => {
  const { deployments, getNamedAccounts } = hre;
  const { deploy } = deployments;
  const { deployer } = await getNamedAccounts();

  const result = await deploy('LabRats', {
    from: deployer,
    args: [],
    gasPrice: hre.ethers.BigNumber.from('1000000000'),
    gasLimit: 8999999,
    log: true,
  });

  console.log(result.address);

  // ADD MORE DEPLOYMENTS HERE
};

func.tags = ['Deploy'];
module.exports = func;
