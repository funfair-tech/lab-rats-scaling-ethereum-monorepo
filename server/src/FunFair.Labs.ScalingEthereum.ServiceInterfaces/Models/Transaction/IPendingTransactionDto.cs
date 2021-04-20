using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Swagger.Attributes;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Transaction
{
    /// <summary>
    ///     Pending transaction details.
    /// </summary>
    public interface IPendingTransactionDto
    {
        /// <summary>
        ///     The transaction hash of the pending transaction.
        /// </summary>
        [SwaggerRequired]
        TransactionHash TransactionHash { get; }

        /// <summary>
        ///     The id of the Ethereum network the transaction was created on.
        /// </summary>
        [SwaggerRequired]
        EthereumNetwork Network { get; }
    }
}