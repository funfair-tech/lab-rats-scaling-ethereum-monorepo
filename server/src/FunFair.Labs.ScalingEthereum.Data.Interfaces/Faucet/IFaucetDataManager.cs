using System.Net;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.Interfaces.Faucet
{
    /// <summary>
    ///     Data manager for faucet access.
    /// </summary>
    public interface IFaucetDataManager
    {
        /// <summary>
        ///     Determines whether the account is allowed to use the faucet.
        /// </summary>
        /// <param name="ipAddress">The IP Address of the user requesting funds from the faucet.</param>
        /// <param name="address">The Account Address of the user requesting funds from the faucet.</param>
        /// <returns>true, if the account is allowed to request funds from the faucet; otherwise, false.</returns>
        Task<bool> IsAllowedToIssueFromFaucetAsync(IPAddress ipAddress, AccountAddress address);

        /// <summary>
        ///     Records a successful issue from the faucet.
        /// </summary>
        /// <param name="recipient">The recipient of the faucet drip.</param>
        /// <param name="nativeCurrencyAmount">The amount of the native currency that was issued.</param>
        /// <param name="tokenAmount">The amount of the token that was issued.</param>
        /// <param name="ipAddress">The requesters IP Address</param>
        Task RecordFundsIssuedAsync(INetworkAccount recipient, EthereumAmount nativeCurrencyAmount, Token tokenAmount, IPAddress ipAddress);
    }
}