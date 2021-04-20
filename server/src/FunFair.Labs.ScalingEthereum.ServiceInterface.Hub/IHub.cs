using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes.Primitives;
using Microsoft.AspNetCore.SignalR;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub
{
    /// <summary>
    ///     A hub
    /// </summary>
    [SuppressMessage(category: "Threading", checkId: "VSTHRD200:Use Async Suffix in names of method that return an awaitable type", Justification = "Hub Method names should not include async")]
    public interface IHub
    {
        /// <summary>
        ///     A message was logged.
        /// </summary>
        /// <param name="connectionId">The connection Id.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        [HubMethodName(HubEventNames.Log)]
        Task Log(string connectionId, string message);

        /// <summary>
        ///     Send the players online data
        /// </summary>
        /// <param name="players">Amount of players</param>
        [HubMethodName(HubEventNames.PlayersOnline)]
        Task PlayersOnline(int players);

        /// <summary>
        ///     Broadcast new chat message.
        /// </summary>
        /// <param name="accountAddress">User's account address.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        [HubMethodName(HubEventNames.NewMessage)]
        Task NewMessage(AccountAddress accountAddress, string message);
    }
}