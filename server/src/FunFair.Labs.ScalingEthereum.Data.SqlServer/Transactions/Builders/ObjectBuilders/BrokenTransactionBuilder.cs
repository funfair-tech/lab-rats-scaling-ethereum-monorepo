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
    ///     Builder of <see cref="BrokenTransaction" />
    /// </summary>
    public sealed class BrokenTransactionBuilder : IObjectBuilder<BrokenTransactionEntity, BrokenTransaction>
    {
        private readonly IEthereumNetworkRegistry _ethereumNetworkRegistry;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkRegistry">Ethereum network registry.</param>
        public BrokenTransactionBuilder(IEthereumNetworkRegistry ethereumNetworkRegistry)
        {
            this._ethereumNetworkRegistry = ethereumNetworkRegistry ?? throw new ArgumentNullException(nameof(ethereumNetworkRegistry));
        }

        /// <inheritdoc />
        public BrokenTransaction? Build(BrokenTransactionEntity? source)
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
            string transactionPriority = source.Priority ?? source.DataError(x => x.Priority);

            return new BrokenTransaction(network: network,
                                         account: source.Account ?? source.DataError(x => x.Account),
                                         contractAddress: source.ContractAddress ?? source.DataError(x => x.ContractAddress),
                                         functionName: source.FunctionName ?? source.DataError(x => x.FunctionName),
                                         transactionHash: source.TransactionHash ?? source.DataError(x => x.TransactionHash),
                                         transactionData: source.TransactionData ?? source.DataError(x => x.TransactionData),
                                         value: source.Value ?? source.DataError(x => x.Value),
                                         gasPrice: source.GasPrice ?? source.DataError(x => x.GasPrice),
                                         gasLimit: source.GasLimit ?? source.DataError(x => x.GasLimit),
                                         nonce: source.Nonce ?? source.DataError(x => x.Nonce),
                                         status: transactionStatus.ToEnum<TransactionStatus>(),
                                         priority: transactionPriority.ToEnum<TransactionPriority>(),
                                         dateCreated: source.DateCreated,
                                         replacedByTransactionId: source.ReplacedByTransactionId,
                                         replacesTransactionId: source.ReplacesTransactionId,
                                         transactionId: source.TransactionId,
                                         retryCount: source.RetryCount);
        }
    }
}