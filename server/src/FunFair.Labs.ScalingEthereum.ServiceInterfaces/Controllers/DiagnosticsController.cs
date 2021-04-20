using System;
using System.Net;
using FunFair.Common.Middleware.Model;
using FunFair.ServerMonitoring;
using FunFair.ServerMonitoring.Statistics;
using FunFair.ServerMonitoring.Statistics.Extensions;
using FunFair.ServerMonitoring.Statistics.Models;
using Microsoft.AspNetCore.Mvc;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Controllers
{
    /// <summary>
    ///     Diagnostics.
    /// </summary>
    public sealed class DiagnosticsController : ApiBaseController
    {
        private readonly IServerStatus _serverStatus;

        /// <summary>
        ///     Constructor,
        /// </summary>
        /// <param name="serverStatus">Server Status</param>
        public DiagnosticsController(IServerStatus serverStatus)
        {
            this._serverStatus = serverStatus ?? throw new ArgumentNullException(nameof(serverStatus));
        }

        /// <summary>
        ///     Gets the networks that are enabled.
        /// </summary>
        /// <returns>The enabled networks</returns>
        [Route("counters")]
        [HttpGet]
        [ProducesResponseType(typeof(StatisticsDto[]), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionDto), (int) HttpStatusCode.InternalServerError)]
        [Produces(contentType: "application/json")]
        public IActionResult GetCounters()
        {
            Statistics result = this._serverStatus.GetCurrent();

            return this.Ok(result.ToDto());
        }
    }
}