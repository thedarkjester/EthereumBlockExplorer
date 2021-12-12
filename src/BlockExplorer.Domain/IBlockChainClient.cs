using BlockExplorer.Domain.Models;
using System.Threading.Tasks;

namespace BlockExplorer.Domain.Clients
{
    public interface IBlockChainClient
    {
        Task<BlockRangeData> GetBlockRangeData(long firstBlock, long lastBlock);
        Task<BlockRangeData> GetRecentBlockRangeData(long howManyRecentBlocks);
    }
}
