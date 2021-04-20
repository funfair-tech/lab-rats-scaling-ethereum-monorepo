using System;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Networks.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Binders
{
    /// <summary>
    ///     Metadata details provider
    /// </summary>
    public sealed class ModelMetadataDetailsProvider : IModelBinderProvider
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ModelMetadataDetailsProvider(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(EthereumNetwork))
            {
                IEthereumNetworkRegistry registry = this._serviceProvider.GetRequiredService<IEthereumNetworkRegistry>();

                (bool success, EthereumNetwork? found) TryParse(string value)
                {
                    return LookupNetworkByName(registry: registry, value: value);
                }

                return new GenericModelBinder<EthereumNetwork>(TryParse);
            }

            return null;
        }

        private static (bool success, EthereumNetwork? found) LookupNetworkByName(IEthereumNetworkRegistry registry, string value)
        {
            if (registry.TryGetByName(name: value, out EthereumNetwork? foundByName))
            {
                return (success: true, found: foundByName);
            }

            return (success: false, found: null);
        }
    }
}