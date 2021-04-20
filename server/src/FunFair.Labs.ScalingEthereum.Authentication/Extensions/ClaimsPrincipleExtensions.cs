using System;
using System.Linq;
using System.Security.Claims;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Authentication.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        private static bool IsRole(this ClaimsPrincipal claimsPrincipal, string role)
        {
            return claimsPrincipal.Claims.Any(predicate: c => c.Type == ClaimTypes.Role && c.Value == role);
        }

        private static string GetClaimValue(this ClaimsPrincipal claimsPrincipal, string type)
        {
            Claim? claim = claimsPrincipal.Claims.SingleOrDefault(predicate: c => c.Type == type);

            if (claim == null)
            {
                throw new ArgumentOutOfRangeException(nameof(type), $"Claim {type} does not exist in JWT");
            }

            return claim.Value;
        }

        public static JwtUser ToJwtUser(this ClaimsPrincipal principal)
        {
            if (!principal.IsRole(Role.Dapp))
            {
                throw new ArgumentOutOfRangeException(nameof(principal), message: "JWT is not in Dapp role");
            }

            if (principal.Identity?.Name == null)
            {
                throw new ArgumentNullException(nameof(principal), message: "Principal identity name is null");
            }

            if (!UserAccountId.TryParse(s: principal.Identity.Name, out UserAccountId userAccountId))
            {
                throw new ArgumentOutOfRangeException(nameof(principal), message: "UserAccountId in JWT is not valid");
            }

            if (!AccountAddress.TryParse(principal.GetClaimValue(JwtOpts.AccountAddressClaim), out AccountAddress? accountAddress))
            {
                throw new ArgumentOutOfRangeException(nameof(principal), message: "AccountAddress in JWT is not valid");
            }

            return new JwtUser(userAccountId: userAccountId, accountAddress: accountAddress);
        }
    }
}