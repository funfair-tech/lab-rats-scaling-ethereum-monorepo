using System;
using System.Diagnostics;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Transactions;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Transactions
{
    /// <summary>
    ///     Input parameters for <seealso cref="EndGameRound" />.
    /// </summary>
    [DebuggerDisplay("{RoundId} {EntropyReveal}")]
    public sealed class EndGameRoundInput : TransactionParameters
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="roundId">Round Id.</param>
        /// <param name="entropyReveal">Entropy Reveal.</param>
        public EndGameRoundInput(GameRoundId roundId, Seed entropyReveal)
        {
            this.RoundId = roundId ?? throw new ArgumentNullException(nameof(roundId));
            this.EntropyReveal = entropyReveal ?? throw new ArgumentNullException(nameof(entropyReveal));
        }

        /// <summary>
        ///     Round Id.
        /// </summary>
        [InputParameter(ethereumDataType: "bytes32", order: 1)]
        public GameRoundId RoundId { get; }

        /// <summary>
        ///     Entropy Reveal.
        /// </summary>
        [InputParameter(ethereumDataType: "bytes32", order: 2)]
        public Seed EntropyReveal { get; }
    }
}