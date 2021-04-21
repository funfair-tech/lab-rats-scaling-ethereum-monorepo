require('@nomiclabs/hardhat-ethers');
require('@nomiclabs/hardhat-waffle');
require('@eth-optimism/hardhat-ovm');
require('hardhat-deploy');
require('dotenv').config();

module.exports = {
  networks: {
    hardhat: {
      accounts: {
        mnemonic: 'test test test test test test test test test test test junk',
      },
    },
    optimism: {
      url: 'https://kovan.optimism.io',
      accounts: [process.env.YOUR_PRIVATE_KEY],
      // This sets the gas price to 0 for all transactions on L2. We do this
      // because account balances are not automatically initiated with an ETH
      // balance (yet, sorry!).
      gasPrice: 0,
      ovm: true, // This sets the network as using the ovm and ensure contract will be compiled against that.
    },
  },
  solidity: '0.8.0',
  ovm: {
    solcVersion: '0.8.0',
  },
  namedAccounts: {
    deployer: 0,
  },
};
