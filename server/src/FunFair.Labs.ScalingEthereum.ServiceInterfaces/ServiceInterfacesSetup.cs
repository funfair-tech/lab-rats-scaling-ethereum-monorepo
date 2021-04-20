using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces
{
    /// <summary>
    ///     Configures the Proxy service interfaces
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceInterfacesSetup
    {
        /// <summary>
        ///     Configures the Service interfaces.
        /// </summary>
        /// <param name="services">The services collection to register services in.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Configure(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
        }
    }
}