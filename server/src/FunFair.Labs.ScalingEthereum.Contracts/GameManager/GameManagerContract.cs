using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events;
using FunFair.Labs.ScalingEthereum.Contracts.GameManager.Transactions;
using FunFair.Labs.ScalingEthereum.Contracts.Networks;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager
{
    /// <summary>
    ///     Game Manager
    /// </summary>
    public static class GameManagerContract
    {
        /// <summary>
        ///     Creates the Game Manager Contract.
        /// </summary>
        /// <returns></returns>
        public static IContractInfo Create()
        {
            return ContractInfoBuilder.Create(WellKnownContracts.GameManager)
                                      .Network(network: Layer2EthereumNetworks.OptimismKovan, new ContractAddress("0xFc35436FecCeC70Ad223dC88B2eba647846F3170"))
                                      .Transaction<StartGameRound>()
                                      .Transaction<EndGameRound>()
                                      .Transaction<EndGameRoundBetting>()
                                      .Event<StartGameRoundEvent>()
                                      .Event<EndGameRoundEvent>()
                                      .Event<NoMoreBetsEvent>()
                                      .Build();
        }
    }
}