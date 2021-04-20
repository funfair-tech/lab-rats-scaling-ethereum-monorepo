using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.Middleware;
using FunFair.Common.Middleware.Helpers;
using FunFair.Common.Middleware.Model;
using FunFair.Ethereum.Blocks.Interfaces;
using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.Contracts.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Ethereum.Wallet.Interfaces;
using FunFair.Labs.ScalingEthereum.Contracts;
using FunFair.Labs.ScalingEthereum.Logic.Exceptions;
using FunFair.Labs.ScalingEthereum.Logic.Faucet;
using FunFair.Labs.ScalingEthereum.Logic.Faucet.Models;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.Extensions;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Faucet;
using Microsoft.AspNetCore.Mvc;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Controllers
{
    /// <summary>
    ///     Faucet controller.
    /// </summary>
    public sealed class FaucetController : ApiBaseController
    {
        private readonly IEthereumAccountManager _ethereumAccountManager;
        private readonly IEthereumBlockStatus _ethereumBlockStatus;
        private readonly IContractInfo _faucetContractInfo;
        private readonly IFaucetManager _faucetManager;
        private readonly IEthereumNetworkRegistry _networkManager;
        private readonly IRemoteIpAddressRetriever _remoteIpAddressRetriever;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="faucetManager">The Faucet manager.</param>
        /// <param name="ethereumAccountManager">The Ethereum Account Manager</param>
        /// <param name="networkManager">Network manager</param>
        /// <param name="requestIpAddressManager">IP Address manager.</param>
        /// <param name="contractInfoRegistry">Contract info registry</param>
        /// <param name="ethereumBlockStatus">Ethereum Block Status</param>
        public FaucetController(IFaucetManager faucetManager,
                                IEthereumAccountManager ethereumAccountManager,
                                IEthereumNetworkRegistry networkManager,
                                IRemoteIpAddressRetriever requestIpAddressManager,
                                IContractInfoRegistry contractInfoRegistry,
                                IEthereumBlockStatus ethereumBlockStatus)
        {
            this._faucetManager = faucetManager ?? throw new ArgumentNullException(nameof(faucetManager));
            this._ethereumAccountManager = ethereumAccountManager ?? throw new ArgumentNullException(nameof(ethereumAccountManager));
            this._networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
            this._remoteIpAddressRetriever = requestIpAddressManager ?? throw new ArgumentNullException(nameof(requestIpAddressManager));
            this._ethereumBlockStatus = ethereumBlockStatus ?? throw new ArgumentNullException(nameof(ethereumBlockStatus));
            this._faucetContractInfo = (contractInfoRegistry ?? throw new ArgumentNullException(nameof(contractInfoRegistry))).FindContractInfo(WellKnownContracts.Faucet);
        }

        /// <summary>
        ///     Transfer Ether and token to an account
        /// </summary>
        /// <remarks>
        ///     Top up a given Ethereum account to a maximum Ether and token balance.
        /// </remarks>
        /// <param name="request">The Request Body</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of pending transactions representing the Ethereum transactions which are transferring token and Ether</returns>
        [HttpPost]
        [ProducesResponseType(typeof(FaucetDripDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionDto), (int) HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ExceptionDto), (int) HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationResultDto), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ServerUnavailableDto), (int) HttpStatusCode.ServiceUnavailable)]
        [Produces(contentType: "application/json")]
        [Route(template: "Open")]
        public Task<IActionResult> OpenFaucetAsync([FromBody] OpenFaucetDto request, CancellationToken cancellationToken)
        {
            return this.RequestFromFaucetAsync(request: request, cancellationToken: cancellationToken);
        }

        private async Task<IActionResult> RequestFromFaucetAsync(OpenFaucetDto request, CancellationToken cancellationToken)
        {
            try
            {
                IPAddress ipAddress = this._remoteIpAddressRetriever.Get(this.HttpContext);

                EthereumNetwork network = request.Network;

                if (!this._faucetContractInfo.IsSupported(network))
                {
                    return ResultHelpers.ExceptionResult(message: "Faucet is not supported on network.", nameof(FaucetController), statusCode: HttpStatusCode.Forbidden);
                }

                INetworkBlockHeader? networkBlockHeader = this._ethereumBlockStatus.GetLatestBlockRetrievedOnNetwork(network);

                if (networkBlockHeader == null)
                {
                    return ResultHelpers.ExceptionResult($"Network {network.Name} is not ready [BNF].", nameof(FaucetController), statusCode: HttpStatusCode.ServiceUnavailable);
                }

                if (this._networkManager.Supported.All(x => x.Name != network.Name))
                {
                    return ResultHelpers.ExceptionResult(message: "Faucet is not supported on network.", nameof(FaucetController), statusCode: HttpStatusCode.Forbidden);
                }

                AccountAddress address = new(request.Address.ToSpan());

                INetworkAccount account = new NetworkAccount(network: network, address: address);

                if (this._ethereumAccountManager.IsAccountConfiguredForUse(account))
                {
                    return ResultHelpers.ExceptionResult(message: "Server cannot give itself token or eth.", nameof(FaucetController), statusCode: HttpStatusCode.Forbidden);
                }

                FaucetDrip drip = await this._faucetManager.OpenAsync(ipAddress: ipAddress, recipient: account, networkBlockHeader: networkBlockHeader, cancellationToken: cancellationToken);

                return ResultHelpers.JsonResult(drip.ToDto(), statusCode: HttpStatusCode.OK);
            }
            catch (TooMuchTokenException)
            {
                return ResultHelpers.ExceptionResult(message: "Too much token!", nameof(FaucetController), statusCode: HttpStatusCode.Forbidden);
            }
            catch (TooFrequentTokenException)
            {
                return ResultHelpers.ExceptionResult(message: "Too much token!", nameof(FaucetController), statusCode: HttpStatusCode.Forbidden);
            }
            catch (InsufficientTokenException)
            {
                return ResultHelpers.ExceptionResult(message: "Insufficient token!", nameof(FaucetController), statusCode: HttpStatusCode.Forbidden);
            }
        }
    }
}