using System.Net;
using FunFair.Common.Middleware.Model;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models;
using Microsoft.AspNetCore.Mvc;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Controllers
{
    /// <summary>
    ///     An Example controller
    /// </summary>
    public sealed class PingController : ApiBaseController
    {
        /// <summary>
        ///     Gets the status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(contentType: "application/json")]
        [ProducesResponseType(typeof(PongDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionDto), (int) HttpStatusCode.InternalServerError)]
        public IActionResult Get()
        {
            PongDto model = new() {Value = "Pong!"};

            return this.Ok(model);
        }
    }
}