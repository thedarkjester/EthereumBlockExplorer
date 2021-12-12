using BlockExplorer.Domain;
using Nethereum.Web3;

namespace BlockExplorer.Clients
{
    public class Web3Provider : IWeb3Provider
    {
        private readonly BlockChainClientConfiguration _clientConfiguration;

        public Web3Provider(BlockChainClientConfiguration clientConfiguration)
        {
            _clientConfiguration = clientConfiguration;
        }

        public IWeb3 GetWeb3()
        {
            return new Web3(_clientConfiguration.RpcAddress);
        }
    }
}
