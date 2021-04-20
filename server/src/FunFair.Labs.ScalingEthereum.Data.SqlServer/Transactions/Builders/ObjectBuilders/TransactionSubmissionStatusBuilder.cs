using System;
using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Exceptions;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Ethereum.Transactions.Data.Interfaces.Models;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="TransactionSubmissionStatus" />
    /// </summary>
    public sealed class TransactionSubmissionStatusBuilder : IObjectBuilder<TransactionSubmissionStatusEntity, TransactionSubmissionStatus>
    {
        private readonly IEthereumNetworkRegistry _ethereumNetworkRegistry;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkRegistry">Ethereum network registry.</param>
        public TransactionSubmissionStatusBuilder(IEthereumNetworkRegistry ethereumNetworkRegistry)
        {
            this._ethereumNetworkRegistry = ethereumNetworkRegistry ?? throw new ArgumentNullException(nameof(ethereumNetworkRegistry));
        }

        /// <inheritdoc />
        public TransactionSubmissionStatus? Build(TransactionSubmissionStatusEntity? source)
        {
            if (source == null)
            {
                return null;
            }

            if (!this._ethereumNetworkRegistry.TryGetByName(source.Network ?? source.DataError(x => x.Network), out EthereumNetwork? network))
            {
                throw new InvalidEthereumNetworkException();
            }

            return new TransactionSubmissionStatus(accountAddress: source.Account ?? source.DataError(x => x.Account),
                                                   network: network,
                                                   totalTransactions: source.TotalTransactions,
                                                   currentNonce: new Nonce(source.CurrentNonce),
                                                   lastMinedNonce: ExtractOptionalNonce(source.LastMinedNonce),
                                                   lastMinedDate: source.LastMinedDate,
                                                   firstUnMinedNonce: ExtractOptionalNonce(source.FirstUnMinedNonce),
                                                   firstUnMinedDate: source.FirstUnMinedDate,
                                                   lastUnMinedNonce: ExtractOptionalNonce(source.LastUnMinedNonce),
                                                   lastUnMinedDate: source.LastUnMinedDate,
                                                   unMinedNonceCount: source.UnMinedNonceCount);
        }

        private static Nonce? ExtractOptionalNonce(long? nonce)
        {
            return nonce.HasValue ? new Nonce(nonce.Value) : null;
        }
    }
}