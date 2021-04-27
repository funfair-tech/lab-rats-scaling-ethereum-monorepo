namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub
{
    /// <summary>
    ///     Public Event names for hub operations.
    /// </summary>
    public static class HubEventNames
    {
        /// <summary>
        ///     Notification that a new chat message occured.
        /// </summary>
        public const string NewMessage = nameof(NewMessage);

        /// <summary>
        ///     Notification that no games available
        /// </summary>
        public const string NoGamesAvailable = nameof(NoGamesAvailable);

        /// <summary>
        ///     Send the players online data
        /// </summary>
        public const string PlayersOnline = nameof(PlayersOnline);

        /// <summary>
        ///     Logging.
        /// </summary>
        public const string Log = nameof(Log);

        /// <summary>
        ///     Game betting history.
        /// </summary>
        public const string History = nameof(History);

        /// <summary>
        ///     Game round startin (TX issued, but not seen)
        /// </summary>
        public const string GameRoundStarting = nameof(GameRoundStarting);

        /// <summary>
        ///     Game round started (TX events seen)
        /// </summary>
        public const string GameRoundStarted = nameof(GameRoundStarted);

        /// <summary>
        ///     Betting is ending (TX issued, but not seen)
        /// </summary>
        public const string BettingEnding = nameof(BettingEnding);

        /// <summary>
        ///     Betting has ended (TX event seen)
        /// </summary>
        public const string BettingEnded = nameof(BettingEnded);

        /// <summary>
        ///     Game ending (TX issued, but not seen)
        /// </summary>
        public const string GameRoundEnding = nameof(GameRoundEnding);

        /// <summary>
        ///     Game has ended (TX event seen)
        /// </summary>
        public const string GameRoundEnded = nameof(GameRoundEnded);

        /// <summary>
        ///     Game has broken
        /// </summary>
        public const string GameRoundBroken = nameof(GameRoundBroken);

        /// <summary>
        ///     Details about the last ended game.
        /// </summary>
        public const string LastGameRoundEnded = nameof(LastGameRoundEnded);

        /// <summary>
        ///     Funds were issued from the faucet.
        /// </summary>
        public const string FaucetDrip = nameof(FaucetDrip);
    }
}