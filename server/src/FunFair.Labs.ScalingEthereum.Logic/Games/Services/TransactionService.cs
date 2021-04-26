using System;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.Contracts.Transactions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Transactions.Data.Interfaces.Models;
using FunFair.Ethereum.Transactions.Interfaces;
using FunFair.Ethereum.Wallet.Interfaces;
using FunFair.Labs.ScalingEthereum.Contracts;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.Services
{
    /// <summary>
    ///     Transaction service
    /// </summary>
    public sealed class TransactionService : ITransactionService
    {
        private readonly IContractInfo _contractInfo;
        private readonly ILogger<TransactionService> _logger;
        private readonly ITransactionExecutorFactory _transactionExecutorFactory;

        /// <summary>
        ///     Constructor,
        /// </summary>
        /// <param name="transactionExecutorFactory">Transaction execution factory.</param>
        /// <param name="contractInfoRegistry">Contract info registry.</param>
        /// <param name="logger">Logging.</param>
        public TransactionService(ITransactionExecutorFactory transactionExecutorFactory, IContractInfoRegistry contractInfoRegistry, ILogger<TransactionService> logger)
        {
            this._transactionExecutorFactory = transactionExecutorFactory ?? throw new ArgumentNullException(nameof(transactionExecutorFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._contractInfo = contractInfoRegistry.FindContractInfo(WellKnownContracts.GameManager);
        }

        /// <inheritdoc />
        public async Task<PendingTransaction> SubmitAsync<TTransactionInput>(INetworkSigningAccount account,
                                                                             TTransactionInput input,
                                                                             TransactionContext transactionContext,
                                                                             CancellationToken cancellationToken)
            where TTransactionInput : TransactionParameters
        {
            this._logger.LogInformation($"{account.Network.Name}: Submit transaction: {typeof(TTransactionInput)}");
            PendingTransaction transaction = await this._contractInfo.SubmitTransactionAsync(transactionExecutorFactory: this._transactionExecutorFactory,
                                                                                             account: account,
                                                                                             input: input,
                                                                                             amountToSend: EthereumAmount.Zero,
                                                                                             priority: TransactionPriority.NORMAL,
                                                                                             context: transactionContext,
                                                                                             cancellationToken: cancellationToken);
            this._logger.LogInformation($"{account.Network.Name}: Transaction submitted: {typeof(TTransactionInput)}, hash: {transaction.TransactionHash}");

            return transaction;
        }
    }
}