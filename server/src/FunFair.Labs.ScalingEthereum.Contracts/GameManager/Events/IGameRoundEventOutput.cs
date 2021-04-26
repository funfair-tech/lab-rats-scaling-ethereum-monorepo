using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events
{
    /// <summary>
    ///     Common interface for game round events
    /// </summary>
    public interface IGameRoundEventOutput
    {
        /// <summary>
        ///     Game round id.
        /// </summary>
        public GameRoundId GameRoundId { get; }
    }
}