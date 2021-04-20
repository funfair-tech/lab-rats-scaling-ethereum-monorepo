namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub
{
    /// <summary>
    ///     Public/Authetnicated Method names for Hub methods
    /// </summary>
    public static class HubMethodNames
    {
        /// <summary>
        ///     Subscribe to multiplayer server events.
        /// </summary>
        public const string Subscribe = nameof(Subscribe);

        /// <summary>
        ///     Send message to all subscribed users
        /// </summary>
        public const string SendMessage = nameof(SendMessage);
    }
}