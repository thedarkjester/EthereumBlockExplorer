using BlockExplorer.Api.Models;
using BlockExplorer.Domain.Clients;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BlockExplorer.Handlers
{
    public class TransferReportResponder : IAnswerTransferReportResponse
    {
        private readonly ILogger<TransferReportResponder> _logger;
        private readonly IBlockChainClient _blockChainClient;
        private readonly IBlockRangeDataMapper _blockRangeDataMapper;

        public TransferReportResponder(ILogger<TransferReportResponder> logger, IBlockChainClient blockChainClient, IBlockRangeDataMapper blockRangeDataMapper)
        {
            _logger = logger;
            _blockChainClient = blockChainClient;
            _blockRangeDataMapper = blockRangeDataMapper;
        }

        public async Task<ApiTransferReportResponse> RetrieveRangedBlockData(long firstBlock, long lastBlock)
        {
            _logger.LogInformation($"Retrieving and mapping data for firstBlock={firstBlock} to lastBlock={lastBlock}");

            var data = await _blockChainClient.GetBlockRangeData(firstBlock, lastBlock);
            var mappedData = _blockRangeDataMapper.MapBlockRangeDataToReportResponse(data);

            return mappedData;
        }

        public async Task<ApiTransferReportResponse> RetrieveCurrentAndPreceedingBlocksData(long howManyRecentBlocks)
        {
            _logger.LogInformation($"Retrieving and mapping data for recentBlocks={howManyRecentBlocks}");

            var data = await _blockChainClient.GetRecentBlockRangeData(howManyRecentBlocks);
            var mappedData = _blockRangeDataMapper.MapBlockRangeDataToReportResponse(data);

            return mappedData;
        }
    }
}
