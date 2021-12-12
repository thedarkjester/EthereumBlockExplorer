using Nethereum.Web3;

namespace BlockExplorer.Clients
{
    public interface IWeb3Provider
    {
        IWeb3 GetWeb3();
    }
}
