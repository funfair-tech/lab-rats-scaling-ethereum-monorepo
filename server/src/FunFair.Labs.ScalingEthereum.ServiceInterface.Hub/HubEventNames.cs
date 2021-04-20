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
    }
}