using FunFair.Common.Environment;
using FunFair.Common.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Random;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription
{
    /// <summary>
    ///     Class which can generate local and global group names
    /// </summary>
    /// <remarks>
    ///     Because some events are raised on all servers in the farm when running on more than one server, and some aren't,
    ///     we need to be able to direct messages to all clients connected to THIS server, or all clients connected to ALL
    ///     servers. This class allows generating local group names by including a random number generated at startup.
    ///     Global group names are global, so don't include the random number.
    ///     If this solution proves inefficient (as all messages will be broadcast to all servers regardless) then a better
    ///     solution can be implemented by storing lists of connection ids against group names and sending local messages
    ///     directly to connection ids instead of groups.
    /// </remarks>
    public sealed class GroupNameGenerator : IGroupNameGenerator
    {
        private readonly ExecutionEnvironment _environment;
        private readonly uint _serverId;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="randomSource">Source of randomness.</param>
        /// <param name="environment">The environment.</param>
        public GroupNameGenerator(IRandomSource randomSource, ExecutionEnvironment environment)
        {
            this._serverId = randomSource.GetUInt32();
            this._environment = environment;
        }

        /// <inheritdoc />
        public string GenerateLocal(EthereumNetwork network, UserAccountId accountAddress)
        {
            return $"{this._environment.GetName()}|{this._serverId}|{network.Name}|{accountAddress}";
        }

        /// <inheritdoc />
        public string GenerateGlobal(EthereumNetwork network, UserAccountId accountAddress)
        {
            return $"{this._environment.GetName()}|{network.Name}|{accountAddress}";
        }

        /// <inheritdoc />
        public string GenerateGlobal(EthereumNetwork network)
        {
            return $"{this._environment.GetName()}|{network.Name}";
        }

        /// <inheritdoc />
        public string GenerateLocal(EthereumNetwork network)
        {
            return $"{this._environment.GetName()}|{this._serverId}|{network.Name}";
        }
    }
}