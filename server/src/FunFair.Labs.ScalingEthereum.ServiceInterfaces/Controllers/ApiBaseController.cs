using Microsoft.AspNetCore.Mvc;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Controllers
{
    /// <summary>
    ///     Base class for API Controllers.
    /// </summary>
    [Route("api/[controller]")]
    public abstract class ApiBaseController : ControllerBase
    {
    }
}