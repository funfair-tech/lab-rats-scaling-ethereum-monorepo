namespace FunFair.Labs.ScalingEthereum.Contracts
{
    /// <summary>
    ///     Environment specific contract configuration.
    /// </summary>
    public enum ContractConfiguration
    {
        /// <summary>
        ///     A development environment - so when on mainnet don't use the production token
        /// </summary>
        DEVELOPMENT,

        /// <summary>
        ///     A production environment - so when on mainnet use the production token
        /// </summary>
        PRODUCTION
    }
}