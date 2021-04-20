namespace FunFair.Labs.ScalingEthereum.Server.ServiceStartup
{
    /// <summary>
    ///     Number of confirmations to wait for.
    /// </summary>
    internal static class Confirmations
    {
        /// <summary>
        ///     General number of confirmation.
        /// </summary>
        public const int StandardTransactionConfirmations = 0;

        /// <summary>
        ///     Confirmations for when a transaction may be at risk of being in an uncle block
        /// </summary>
        public const int HighRiskTransactionConfirmations = 5;
    }
}