using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.Contracts.Transactions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Transactions.Data.Interfaces.Models;
using FunFair.Ethereum.Wallet.Interfaces;

namespace FunFair.Labs.ScalingEthereum.Logic.Games
{
    /// <summary>
    ///     Transaction service
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        ///     Submit given transaction and wait to be executed
        /// </summary>
        /// <param name="account">The network to submit the transaction with.</param>
        /// <param name="input">Transaction input</param>
        /// <param name="transactionContext">The transaction context</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <typeparam name="TTransactionInput">Type of transaction</typeparam>
        /// <returns></returns>
        Task<PendingTransaction> SubmitAsync<TTransactionInput>(INetworkSigningAccount account, TTransactionInput input, TransactionContext transactionContext, CancellationToken cancellationToken)
            where TTransactionInput : TransactionParameters;
    }
}