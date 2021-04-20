using System.Diagnostics;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Swagger.Attributes;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Faucet
{
    /// <summary>
    ///     Request for getting funds from the faucet.
    /// </summary>
    [DebuggerDisplay(value: "NetID: {Network} Address: {Address}")]
    public sealed class OpenFaucetDto
    {
        /// <summary>
        ///     The id of the Ethereum network to open the Faucet on.
        /// </summary>
        [SwaggerRequired]
        public EthereumNetwork Network { get; init; } = default!;

        /// <summary>
        ///     The Ethereum account to open the Faucet for.
        /// </summary>
        [SwaggerRequired]
        public HexAddress Address { get; init; } = default!;
    }
}