using FunFair.Ethereum.Contracts;
using FunFair.Labs.ScalingEthereum.Contracts.Faucet;
using FunFair.Labs.ScalingEthereum.Contracts.Token;

namespace FunFair.Labs.ScalingEthereum.Contracts.Services
{
    /// <summary>
    ///     Registry of <see cref="IContractInfo" />
    /// </summary>
    public sealed class ContractInfoRegistry : BaseContractInfoRegistry
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public ContractInfoRegistry()
            : base(new[] {TokenContract.Create(), FaucetContract.Create()})
        {
        }
    }
}