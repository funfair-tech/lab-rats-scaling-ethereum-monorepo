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
                                      .Network(network: Layer2EthereumNetworks.OptimismKovan, new ContractAddress("0xe3f2Fa6a3F16837d012e1493F50BD29db0BdADe4"))
                                      .Transaction<StartGameRound>()
                                      .Transaction<StopBetting>()
                                      .Transaction<EndGameRound>()
                                      .Event<StartGameRoundEvent>()
                                      .Event<EndGameRoundEvent>()
                                      .Event<NoMoreBetsEvent>()
                                      .Build();
        }
    }
}