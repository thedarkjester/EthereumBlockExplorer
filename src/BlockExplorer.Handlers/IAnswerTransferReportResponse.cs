using BlockExplorer.Api.Models;
using System.Threading.Tasks;

namespace BlockExplorer.Handlers
{
    public interface IAnswerTransferReportResponse
    {
        Task<ApiTransferReportResponse> RetrieveRangedBlockData(long firstBlock, long lastBlock);
        Task<ApiTransferReportResponse> RetrieveCurrentAndPreceedingBlocksData(long howManyRecentBlocks);
    }
}