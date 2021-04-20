using FunFair.Labs.ScalingEthereum.Logic.Faucet.Models;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Faucet;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Extensions
{
    /// <summary>
    ///     Faucet drip extensions
    /// </summary>
    public static class FaucetDripExtensions
    {
        /// <summary>
        ///     To faucet drip dto
        /// </summary>
        /// <param name="drip">Source </param>
        /// <returns>Dto</returns>
        public static FaucetDripDto ToDto(this FaucetDrip drip)
        {
            return new(ethAmount: drip.EthAmount, tokenAmount: drip.TokenAmount, drip.Transaction.ToDto());
        }
    }
}