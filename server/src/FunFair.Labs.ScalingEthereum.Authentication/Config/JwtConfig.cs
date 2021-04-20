namespace FunFair.Labs.ScalingEthereum.Authentication.Config
{
    /// <summary>
    ///     Jwt config
    /// </summary>
    public sealed class JwtConfig : IJwtConfig
    {
        /// <summary>
        ///     Public key for app jwt signing
        /// </summary>
        public byte[]? EcDsaPublicKey { get; init; }

        /// <summary>
        ///     Audience
        /// </summary>
        public string Audience { get; init; } = default!;

        /// <summary>
        ///     Issuer
        /// </summary>
        public string Issuer { get; init; } = default!;
    }
}