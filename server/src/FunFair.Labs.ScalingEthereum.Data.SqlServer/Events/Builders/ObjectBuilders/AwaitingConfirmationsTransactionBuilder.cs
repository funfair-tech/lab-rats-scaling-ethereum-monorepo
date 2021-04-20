using System;
using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Exceptions;
using FunFair.Ethereum.Events.Data.Interfaces.Models;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="AwaitingConfirmationsTransaction" /> from <see cref="AwaitingConfirmationsTransactionEntity" />.
    /// </summary>
    public sealed class AwaitingConfirmationsTransactionBuilder : IObjectBuilder<AwaitingConfirmationsTransactionEntity, AwaitingConfirmationsTransaction>
    {
        private readonly IEthereumNetworkRegistry _ethereumNetworkRegistry;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkRegistry">Ethereum Network Registry</param>
        public AwaitingConfirmationsTransactionBuilder(IEthereumNetworkRegistry ethereumNetworkRegistry)
        {
            this._ethereumNetworkRegistry = ethereumNetworkRegistry ?? throw new ArgumentNullException(nameof(ethereumNetworkRegistry));
        }

        /// <inheritdoc />
        public AwaitingConfirmationsTransaction? Build(AwaitingConfirmationsTransactionEntity? source)
        {
            if (source == null)
            {
                return null;
            }

            if (!this._ethereumNetworkRegistry.TryGetByName(source.Network ?? source.DataError(x => x.Network), out EthereumNetwork? network))
            {
                throw new InvalidEthereumNetworkException();
            }

            return new AwaitingConfirmationsTransaction(transactionHash: source.TransactionHash ?? source.DataError(x => x.TransactionHash),
                                                        contractAddress: source.ContractAddress ?? source.DataError(x => x.ContractAddress),
                                                        eventSignature: source.EventSignature ?? source.DataError(x => x.EventSignature),
                                                        eventIndex: source.EventIndex,
                                                        gasUsed: source.GasUsed,
                                                        gasPrice: source.GasPrice ?? source.DataError(x => x.GasPrice),
                                                        network: network,
                                                        blockNumberFirstSeen: source.BlockNumberFirstSeen ?? source.DataError(x => x.BlockNumberFirstSeen),
                                                        confirmationsToWaitFor: source.ConfirmationsToWaitFor);
        }
    }
}