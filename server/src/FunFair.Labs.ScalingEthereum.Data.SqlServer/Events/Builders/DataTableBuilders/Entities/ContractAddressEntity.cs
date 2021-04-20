using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities
{
    /// <summary>
    ///     An entity that contains a contract address,
    /// </summary>
    [DebuggerDisplay("{" + nameof(ContractAddress) + "}")]
    public sealed record ContractAddressEntity
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="contractAddress">The contract address.</param>
        public ContractAddressEntity(ContractAddress contractAddress)
        {
            this.ContractAddress = contractAddress ?? throw new ArgumentNullException(nameof(contractAddress));
        }

        /// <summary>
        ///     The contract address.
        /// </summary>
        public ContractAddress ContractAddress { get; }
    }
}