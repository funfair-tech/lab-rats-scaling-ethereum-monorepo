using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Authentication
{
    /// <summary>
    ///     Jwt user
    /// </summary>
    public sealed class JwtUser
    {
        public JwtUser(UserAccountId userAccountId, AccountAddress accountAddress)
        {
            this.Id = userAccountId;
            this.AccountAddress = accountAddress;
        }

        /// <summary>
        ///     User account id
        /// </summary>
        public UserAccountId Id { get; }

        /// <summary>
        ///     Account address
        /// </summary>
        public AccountAddress AccountAddress { get; }
    }
}