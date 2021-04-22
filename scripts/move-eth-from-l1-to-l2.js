require('dotenv').config();

const { Contract, Wallet } = require('ethers');
const { JsonRpcProvider } = require('@ethersproject/providers');

const KOVAN_CONTRACT_ADDRESS = '0xf3902e50dA095bD2e954AB320E8eafDA6152dFDa';
const ABI =
  '[{"inputs":[],"stateMutability":"nonpayable","type":"constructor"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"_from","type":"address"},{"indexed":false,"internalType":"address","name":"_to","type":"address"},{"indexed":false,"internalType":"uint256","name":"_amount","type":"uint256"}],"name":"DepositInitiated","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"_to","type":"address"},{"indexed":false,"internalType":"uint256","name":"_amount","type":"uint256"}],"name":"WithdrawalFinalized","type":"event"},{"inputs":[],"name":"deposit","outputs":[],"stateMutability":"payable","type":"function"},{"inputs":[{"internalType":"address","name":"_to","type":"address"}],"name":"depositTo","outputs":[],"stateMutability":"payable","type":"function"},{"inputs":[{"internalType":"address","name":"_to","type":"address"},{"internalType":"uint256","name":"_amount","type":"uint256"}],"name":"finalizeWithdrawal","outputs":[],"stateMutability":"nonpayable","type":"function"},{"inputs":[],"name":"getFinalizeDepositL2Gas","outputs":[{"internalType":"uint32","name":"","type":"uint32"}],"stateMutability":"view","type":"function"},{"inputs":[{"internalType":"address","name":"_libAddressManager","type":"address"},{"internalType":"address","name":"_ovmEth","type":"address"}],"name":"initialize","outputs":[],"stateMutability":"nonpayable","type":"function"},{"inputs":[],"name":"libAddressManager","outputs":[{"internalType":"contract Lib_AddressManager","name":"","type":"address"}],"stateMutability":"view","type":"function"},{"inputs":[],"name":"messenger","outputs":[{"internalType":"address","name":"","type":"address"}],"stateMutability":"view","type":"function"},{"inputs":[],"name":"ovmEth","outputs":[{"internalType":"address","name":"","type":"address"}],"stateMutability":"view","type":"function"},{"inputs":[{"internalType":"string","name":"_name","type":"string"}],"name":"resolve","outputs":[{"internalType":"address","name":"_contract","type":"address"}],"stateMutability":"view","type":"function"},{"stateMutability":"payable","type":"receive"}]';

async function main() {
  // Set up our RPC provider connections.
  const l1RpcProvider = new JsonRpcProvider(
    'https://kovan.infura.io/v3/9aa3d95b3bc440fa88ea12eaa4456161'
  );
  // const l2RpcProvider = new JsonRpcProvider('https://kovan.optimism.io');

  const key = process.env.YOUR_PRIVATE_KEY;
  const l1Wallet = new Wallet(key, l1RpcProvider);
  // const l2Wallet = new Wallet(key, l2RpcProvider);

  const contract = new Contract(KOVAN_CONTRACT_ADDRESS, ABI, l1Wallet);
  // we use this one so you can change address if you want
  const response = await contract.depositTo(l1Wallet.address, {
    value: '100000000000000',
  });
  console.log(response);
  await response.wait();
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
