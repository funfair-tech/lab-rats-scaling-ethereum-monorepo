using System;

namespace FunFair.Labs.ScalingEthereum.DataTypes.Wallet
{
    /// <summary>
    ///     Wallet configuration
    /// </summary>
    public sealed class WalletConfiguration
    {
        /// <summary>
        ///     Url to the wallet SDK (embedded in the client).
        /// </summary>
        public Uri SdkUri { get; init; } = default!;

        public string AppId { get; init; } = default!;
    }
}