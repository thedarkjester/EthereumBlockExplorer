using BlockExplorer.Api.Models;
using BlockExplorer.Domain.Models;
using System.Linq;
using System.Collections.Generic;

namespace BlockExplorer.Handlers
{
    public class BlockRangeDataToApiModelMapper : IBlockRangeDataMapper
    {
        public ApiTransferReportResponse MapBlockRangeDataToReportResponse(BlockRangeData blockRangeData)
        {

            Dictionary<bool, IEnumerable<ApiAddressTotals>> addressTotals = MapBlockData(blockRangeData);

            ApiTransferReportResponse response = new ApiTransferReportResponse
            {
                TotalTransfered = blockRangeData.TransferTotal,
                FirstBlock = blockRangeData.FirstBlockNumber,
                LastBlock = blockRangeData.LastBlockNumber,
                ContractsCreated = blockRangeData.ContractsCreatedCount,
                AddressTransferTotals = addressTotals
            };

            return response;
        }

        private Dictionary<bool, IEnumerable<ApiAddressTotals>> MapBlockData(BlockRangeData blockRangeData)
        {
            var ApiTotals = blockRangeData.AddressTransferTotals.Select(addressData => new ApiAddressTotals
            {
                Address = addressData.Address,
                FirstBlockSeenInRange = addressData.FirstTransactionBlock,
                LatestBlockSeenInRange = addressData.LastTransactionBlock,
                Received = addressData.Received,
                Sent = addressData.Sent,
                IsContractAddress = addressData.IsContract,
                TransactionsSeenInCount = addressData.TransactionsSeenInCount
            })
                .GroupBy(x => x.IsContractAddress)
                .ToDictionary(x => x.Key, y => y.AsEnumerable());

            return ApiTotals;
        }
    }
}
