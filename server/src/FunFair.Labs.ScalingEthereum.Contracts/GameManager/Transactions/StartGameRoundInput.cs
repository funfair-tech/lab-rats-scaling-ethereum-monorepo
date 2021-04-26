using System;
using System.Diagnostics;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Transactions;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Transactions
{
    /// <summary>
    ///     Input parameters for <seealso cref="StartGameRound" />.
    /// </summary>
    [DebuggerDisplay("{RoundId} Game Contract: {GameAddress}")]
    public sealed class StartGameRoundInput : TransactionParameters
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="roundId">Round Id.</param>
        /// <param name="gameAddress">The Game Contract Address.</param>
        /// <param name="entropyCommit">Entropy Commit.</param>
        public StartGameRoundInput(GameRoundId roundId, ContractAddress gameAddress, Seed entropyCommit)
        {
            this.RoundId = roundId ?? throw new ArgumentNullException(nameof(roundId));
            this.GameAddress = gameAddress ?? throw new ArgumentNullException(nameof(gameAddress));
            this.EntropyCommit = entropyCommit ?? throw new ArgumentNullException(nameof(entropyCommit));
        }

        /// <summary>
        ///     Round Id.
        /// </summary>
        [InputParameter(ethereumDataType: "bytes32", order: 1)]
        public GameRoundId RoundId { get; }

        /// <summary>
        ///     The Game Contract Address.
        /// </summary>
        [InputParameter(ethereumDataType: "address", order: 2)]
        public ContractAddress GameAddress { get; }

        /// <summary>
        ///     Entropy Commit.
        /// </summary>
        [InputParameter(ethereumDataType: "bytes32", order: 3)]
        public Seed EntropyCommit { get; }
    }
}