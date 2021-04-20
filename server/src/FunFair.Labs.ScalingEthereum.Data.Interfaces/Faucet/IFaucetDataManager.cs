using System.Net;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes.Primitives;

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
    }
}