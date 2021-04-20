using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Contracts.Faucet.Events;
using FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions;
using FunFair.Labs.ScalingEthereum.Contracts.Networks;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet
{
    /// <summary>
    ///     The faucet contract.
    /// </summary>
    public static class FaucetContract
    {
        /// <summary>
        ///     Creates the faucet.
        /// </summary>
        /// <returns></returns>
        public static IContractInfo Create()
        {
            // TODO: Add contract addresses here once deployed
            return ContractInfoBuilder.Create(WellKnownContracts.Faucet)
                                      .Network(network: Layer2EthereumNetworks.OptimismKovan, new ContractAddress("0x1111111111111111111111111111111111111111"))
                                      .Transaction<WithdrawEth>()
                                      .Transaction<WithdrawToken>()
                                      .Transaction<DistributeEth>()
                                      .Transaction<DistributeToken>()
                                      .Transaction<DistributeTokenAndEth>()
                                      .Event<DistributedEth>()
                                      .Event<DistributedToken>()
                                      .Event<SentFundsToContract>()
                                      .Event<WithdrewEthFromContract>()
                                      .Event<WithdrewTokenFromContract>()
                                      .Build();
        }
    }
}