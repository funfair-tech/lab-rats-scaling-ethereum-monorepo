using System.Diagnostics;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Swagger.Attributes;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Transaction
{
    /// <summary>
    ///     Pending transaction details.
    /// </summary>
    [DebuggerDisplay(value: "Network: {Network} TransactionHash: {TransactionHash}")]
    public sealed class PendingTransactionDto : IPendingTransactionDto
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="network">The id of the Ethereum network the transaction was created on.</param>
        /// <param name="transactionHash">The transaction hash of the pending transaction.</param>
        public PendingTransactionDto(EthereumNetwork network, TransactionHash transactionHash)
        {
            this.Network = network;
            this.TransactionHash = transactionHash;
        }

        /// <inheritdoc />
        [SwaggerRequired]
        public TransactionHash TransactionHash { get; }

        /// <inheritdoc />
        [SwaggerRequired]
        public EthereumNetwork Network { get; }
    }
}