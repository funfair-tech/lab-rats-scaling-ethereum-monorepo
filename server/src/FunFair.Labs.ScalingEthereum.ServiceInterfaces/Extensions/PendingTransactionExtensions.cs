using FunFair.Ethereum.DataTypes;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Transaction;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Extensions
{
    /// <summary>
    ///     Pending transaction extensions
    /// </summary>
    public static class PendingTransactionExtensions
    {
        /// <summary>
        ///     To pending transaction dto
        /// </summary>
        /// <param name="transaction">Source</param>
        /// <returns>Dto</returns>
        public static PendingTransactionDto ToDto(this PendingTransaction transaction)
        {
            return new(transactionHash: transaction.TransactionHash, network: transaction.Network);
        }
    }
}