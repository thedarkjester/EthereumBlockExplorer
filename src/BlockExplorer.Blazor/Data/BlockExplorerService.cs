using BlockExplorer.Api.Models;
using BlockExplorer.Handlers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlockExplorer.Blazor.Data
{
    public class BlockExplorerService
    {
        private readonly IAnswerTransferReportResponse _transferReportResponder;

        public BlockExplorerService(IAnswerTransferReportResponse transferReportResponder)
        {
            _transferReportResponder = transferReportResponder;
        }


        public async Task<ApiTransferReportResponse> GetTransferReport(ExplorerParams explorerParams)
        {
            // try parse should be used- just getting this working.
            var reportResponse = await _transferReportResponder.RetrieveRangedBlockData(explorerParams.Start, explorerParams.End);

            return reportResponse;
        }

        public async Task<ApiTransferReportResponse> GetTransferReportByPreceedingBlocks(ExplorerParams explorerParams)
        {
            // try parse should be used- just getting this working.
            var reportResponse = await _transferReportResponder.RetrieveCurrentAndPreceedingBlocksData(explorerParams.PreceedingBlocks);

            return reportResponse;
        }
    }
}
