using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
        public EthereumNetwork Network { get; set; } = default!;

        /// <summary>
        ///     The Ethereum account to open the Faucet for.
        /// </summary>
        [SwaggerRequired]
        [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
        public HexAddress Address { get; set; } = default!;
    }
}