using BlockExplorer.Clients;
using BlockExplorer.Domain;
using BlockExplorer.Domain.Clients;
using BlockExplorer.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockExplorer.Blazor
{
    public static class DependencyInjectionConfiguration
    {
        public static void ConfigureDependencyInjection(IConfiguration configuration, IServiceCollection services)
        {
            AddBlockChainClients(configuration, services);
            RegisterCustomServices(services);
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
