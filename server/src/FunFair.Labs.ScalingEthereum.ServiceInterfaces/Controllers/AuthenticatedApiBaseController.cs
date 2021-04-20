using FunFair.Labs.ScalingEthereum.Authentication;
using FunFair.Labs.ScalingEthereum.Authentication.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Controllers
{
    /// <summary>
    ///     Base class for authenticated API Controllers.
    /// </summary>
    [Authorize]
    public abstract class AuthenticatedApiBaseController : ApiBaseController
    {
        /// <summary>
        ///     Jwt user
        /// </summary>
        protected JwtUser? JwtUser
        {
            get
            {
                // TODO : Look to see if can do something similar to the wallet and throw exception etc.
                if (this.User?.Identity == null)
                {
                    return null;
                }

                if (!this.User.Identity.IsAuthenticated)
                {
                    return null;
                }

                return this.User.ToJwtUser();
            }
        }
    }
}