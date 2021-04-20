namespace FunFair.Labs.ScalingEthereum.Authentication
{
    /// <summary>
    ///     Jwt token options
    /// </summary>
    public static class JwtOpts
    {
        /// <summary>
        ///     User's unique name
        /// </summary>
        public static string UniqueName { get; } = "unique_name";

        /// <summary>
        ///     Account address claim
        /// </summary>
        public static string AccountAddressClaim => "addr";
    }
}