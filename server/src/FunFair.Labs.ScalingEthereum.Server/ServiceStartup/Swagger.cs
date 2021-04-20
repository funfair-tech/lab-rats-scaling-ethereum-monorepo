using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

#if SWAGGER_ENABLED
using System.IO;
using FunFair.Common.DataTypes;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Swagger.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
#endif

namespace FunFair.Labs.ScalingEthereum.Server.ServiceStartup
{
    internal static class Swagger
    {
        [SuppressMessage(category: "Microsoft.Usage", checkId: "CA1801:ReviewUnusedParameters", Justification = "Interface defined for when swagger is enabled.")]
        public static IServiceCollection ConfigureSwaggerServices(this IServiceCollection services, string version)
        {
#if SWAGGER_ENABLED
            return services.AddSwaggerGen(setupAction: c =>
                                                       {
                                                           void ConfigureXmlDoc(string fileName)
                                                           {
                                                               string docPath = Path.Combine(path1: ApplicationConfigLocator.ConfigurationFilesPath, path2: fileName);

                                                               if (File.Exists(docPath))
                                                               {
                                                                   c.IncludeXmlComments(docPath);
                                                               }
                                                           }

                                                           c.SwaggerDoc(name: "v1", new OpenApiInfo {Title = $"Labs Scaling Ethereum Server API {version}", Version = version});

                                                           ConfigureXmlDoc("FunFair.Labs.ScalingEthereum.ServiceInterfaces.xml");

                                                           c.AddLowerCaseDocumentFilter();
                                                           c.SchemaFilter<RemoveTrailingPeriodsFilter>();
                                                           c.AddRemoveFakeParametersInPathDocumentFilter();
                                                           c.CustomSchemaIds(x => x.FullName);
                                                           c.AddAttributedDocumentationExtensions();

                                                           MapVariableLengthHexStringType<Token>(options: c, maxLength: Token.MaxStringLength);
                                                           MapFixedLengthHexStringType<EthereumAddress>(options: c, length: EthereumAddress.RequiredStringLength);
                                                           MapFixedLengthHexStringType<Signature>(options: c, length: Signature.RequiredStringLength);
                                                           MapFixedLengthHexStringType<AccountAddress>(options: c, length: EthereumAddress.RequiredStringLength);
                                                           MapFixedLengthHexStringType<TransactionHash>(options: c, length: TransactionHash.RequiredStringLength);
                                                           MapFixedLengthHexStringType<BlockHash>(options: c, length: BlockHash.RequiredStringLength);
                                                           MapFixedLengthHexStringType<HexAddress>(options: c, length: HexAddress.RequiredStringLength);

                                                           MapFixedLengthHexStringType<ContractAddress>(options: c, length: EthereumAddress.RequiredStringLength);

                                                           MapVariableLengthHexStringType<UnsignedHexInteger>(options: c, maxLength: UnsignedHexInteger.MaximumStringLength);
                                                           MapVariableLengthHexStringType<GasPrice>(options: c, maxLength: GasPrice.MaximumStringLength);
                                                           MapVariableLengthHexStringType<GasLimit>(options: c, maxLength: GasLimit.MaximumStringLength);
                                                           MapVariableLengthHexStringType<Nonce>(options: c, maxLength: Nonce.MaximumStringLength);
                                                           MapVariableLengthHexStringType<EthereumAmount>(options: c, maxLength: EthereumAmount.MaximumStringLength);
                                                           MapVariableLengthHexStringType<BlockNumber>(options: c, maxLength: BlockNumber.MaximumStringLength);
                                                           MapFixedLengthHexStringType<EventTopic>(options: c, length: EventTopic.RequiredStringLength);
                                                           MapFixedLengthHexStringType<EventSignature>(options: c, length: EventSignature.RequiredStringLength);
                                                           MapFixedLengthHexStringType<LogsBloom>(options: c, length: LogsBloom.RequiredStringLength);

                                                           MapUnboundedHexStringType<TransactionData>(options: c);

                                                           MapUnboundedHexStringType<CallData>(c);
                                                           MapUnboundedHexStringType<CallOutputData>(c);
                                                           MapUnboundedHexStringType<SignedTransaction>(c);
                                                           MapUnboundedHexStringType<EventData>(c);

                                                           MapVariableLengthHexStringType<TokenAmount>(options: c, maxLength: TokenAmount.MaximumStringLength);

                                                           c.MapType<EthereumNetwork>(
                                                               schemaFactory: () => new OpenApiSchema {Type = "string", MaxLength = EthereumAmount.MaximumStringLength});
                                                       });
#else

            // Swagger not enabled
            return services;
#endif
        }

        [SuppressMessage(category: "Microsoft.Usage", checkId: "CA1801:ReviewUnusedParameters", Justification = "Interface defined for when swagger is enabled.")]
        public static void RegisterSwagger(IApplicationBuilder app, string version)
        {
#if SWAGGER_ENABLED
            app.UseSwagger()
               .UseSwaggerUI(setupAction: c => { c.SwaggerEndpoint(url: "v1/swagger.json", $"FunFair-Labs-MultiPlayer-Server {version}"); });
#else

            // Swagger not enabled
#endif
        }

#if SWAGGER_ENABLED
        private static void MapUnboundedHexStringType<T>(SwaggerGenOptions options)
        {
            options.MapType<T>(schemaFactory: () => new OpenApiSchema {Type = "string", Format = "hex-string"});
        }

        private static void MapVariableLengthHexStringType<T>(SwaggerGenOptions options, uint maxLength)
        {
            options.MapType<T>(schemaFactory: () => new OpenApiSchema {Type = "string", Format = "hex-string", MaxLength = (int) maxLength});
        }

        private static void MapFixedLengthHexStringType<T>(SwaggerGenOptions options, uint length)
        {
            options.MapType<T>(schemaFactory: () => new OpenApiSchema {Type = "string", Format = "hex-string", MinLength = (int) length, MaxLength = (int) length});
        }

#endif
    }
}