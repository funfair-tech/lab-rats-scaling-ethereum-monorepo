using System;
using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Extensions;
using FunFair.Common.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Exceptions;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="PendingTransaction" /> objects.
    /// </summary>
    public sealed class PendingTransactionBuilder : IObjectBuilder<PendingTransactionEntity, PendingTransaction>
    {
        private readonly IEthereumNetworkRegistry _ethereumNetworkRegistry;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkRegistry">Ethereum network registry.</param>
        public PendingTransactionBuilder(IEthereumNetworkRegistry ethereumNetworkRegistry)
        {
            this._ethereumNetworkRegistry = ethereumNetworkRegistry ?? throw new ArgumentNullException(nameof(ethereumNetworkRegistry));
        }

        /// <inheritdoc />
        public PendingTransaction? Build(PendingTransactionEntity? source)
        {
            if (source == null)
            {
                return null;
            }

            if (!this._ethereumNetworkRegistry.TryGetByName(source.Network ?? source.DataError(x => x.Network), out EthereumNetwork? network))
            {
                throw new InvalidEthereumNetworkException();
            }

            string transactionStatus = source.Status ?? source.DataError(x => x.Status);

            return new PendingTransaction(dateSubmitted: source.DateSubmitted,
                                          network: network,
                                          accountAddress: source.Account ?? source.DataError(x => x.Account),
                                          transactionHash: source.TransactionHash ?? source.DataError(x => x.TransactionHash),
                                          gasLimit: source.GasLimit ?? source.DataError(x => x.GasLimit),
                                          gasPrice: source.GasPrice ?? source.DataError(x => x.GasPrice),
                                          nonce: source.Nonce ?? source.DataError(x => x.Nonce),
                                          gasPolicyExecution: source.GasPolicyExecution,
                                          status: transactionStatus.ToEnum<TransactionStatus>(),
                                          retryCount: source.RetryCount,
                                          dateLastRetried: source.DateLastRetried);
        }
    }
}