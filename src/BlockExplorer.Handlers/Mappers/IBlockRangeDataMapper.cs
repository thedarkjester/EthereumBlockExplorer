using BlockExplorer.Api.Models;
using BlockExplorer.Domain.Models;

namespace BlockExplorer.Handlers
{
    public interface IBlockRangeDataMapper
    {
        ApiTransferReportResponse MapBlockRangeDataToReportResponse(BlockRangeData blockRangeData);
    }
}
