using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using BlockExplorer.Handlers;
using BlockExplorer.Domain.Clients;
using BlockExplorer.Clients;
using BlockExplorer.Domain;

namespace BlockExplorer.Api
{
    public static class DependencyInjectionConfiguration
    {
        public static void ConfigureDependencyInjection(IConfiguration configuration, IServiceCollection services)
        {
            AddBlockChainClients(configuration, services);
            RegisterCustomServices(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlockExporer", Version = "v1" });
            });
        }

        private static void AddBlockChainClients(IConfiguration configuration, IServiceCollection services)
        {
            services.AddTransient<IBlockChainClient, NethereumClient>();

            services.AddSingleton(new BlockChainClientConfiguration
            {
                RpcAddress = configuration.GetValue<string>("RpcAddress")
            }); ;

            services.AddSingleton<IWeb3Provider, Web3Provider>();
        }

        private static void RegisterCustomServices(IServiceCollection services)
        {
            services.AddTransient<IAnswerTransferReportResponse, TransferReportResponder>();
            services.AddSingleton<IBlockRangeDataMapper, BlockRangeDataToApiModelMapper>();
        }
    }
}
