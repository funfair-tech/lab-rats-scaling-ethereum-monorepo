using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound
{
    /// <summary>
    ///     End round win amount
    /// </summary>
    public sealed class WinAmount
    {
        /// <summary>
        ///     Player address
        /// </summary>
        public AccountAddress AccountAddress { get; init; } = default!;

        /// <summary>
        ///     Winning amount
        /// </summary>
        public Token Amount { get; init; } = default!;
    }
}