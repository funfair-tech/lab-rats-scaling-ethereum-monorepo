using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Transaction;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Faucet
{
    /// <summary>
    ///     An issuance from the faucet.
    /// </summary>
    public interface IFaucetDripDto<out TTransactionDto>
        where TTransactionDto : IPendingTransactionDto
    {
        /// <summary>
        ///     The amount of ETH that was issued.
        /// </summary>
        public EthereumAmount EthAmount { get; }

        /// <summary>
        ///     The amount of FUN that was issued.
        /// </summary>
        public Token TokenAmount { get; }

        /// <summary>
        ///     The transaction that was issued to transfer the funds.
        /// </summary>
        public TTransactionDto Transaction { get; }
    }
}