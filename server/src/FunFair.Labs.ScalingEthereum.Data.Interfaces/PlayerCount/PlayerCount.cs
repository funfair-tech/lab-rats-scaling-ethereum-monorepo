using System.Diagnostics;
using FunFair.Ethereum.DataTypes;

namespace FunFair.Labs.ScalingEthereum.Data.Interfaces.PlayerCount
{
    /// <summary>
    ///     A game round
    /// </summary>
    [DebuggerDisplay("Network: {Network} Machine: {MachineName}, count: {Count}")]
    public sealed class PlayerCount
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public PlayerCount(string machineName, EthereumNetwork network, int count)
        {
            this.MachineName = machineName;
            this.Network = network;
            this.Count = count;
        }

        /// <summary>
        ///     The machine name
        /// </summary>
        public string MachineName { get; }

        /// <summary>
        ///     The network
        /// </summary>
        public EthereumNetwork Network { get; }

        /// <summary>
        ///     The total number of players
        /// </summary>
        public int Count { get; }
    }
}