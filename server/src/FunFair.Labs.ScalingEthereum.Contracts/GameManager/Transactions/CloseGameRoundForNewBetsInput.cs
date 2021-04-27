using System;
using System.Diagnostics;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Transactions;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Transactions
{
    /// <summary>
    ///     Input parameters for <seealso cref="CloseGameRoundForNewBets" />.
    /// </summary>
    [DebuggerDisplay("{" + nameof(RoundId) + "}")]
    public sealed class CloseGameRoundForNewBetsInput : TransactionParameters
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="roundId">Round Id.</param>
        public CloseGameRoundForNewBetsInput(GameRoundId roundId)
        {
            this.RoundId = roundId ?? throw new ArgumentNullException(nameof(roundId));
        }

        /// <summary>
        ///     Round Id.
        /// </summary>
        [InputParameter(ethereumDataType: "bytes32", order: 1)]
        public GameRoundId RoundId { get; }
    }
}