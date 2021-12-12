using BlockExplorer.Domain.Models;
using BlockExplorer.Handlers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BlockExplorer.Tests.UnitTests
{
    public class MapBlockRangeDataToApiModelTests
    {
        private readonly BlockRangeDataToApiModelMapper _sut;

        private readonly string testContractAddress = "0x45a36a8e118c37e4c47ef4ab827a7c9e579e11e2";
        private readonly string testExternallyOwnedAccountAddress = "0x3aB28eCeDEa6cdb6feeD398E93Ae8c7b316B1182";

        public MapBlockRangeDataToApiModelTests()
        {
            _sut = new BlockRangeDataToApiModelMapper();
        }

        [Fact]
        public void Mapper_Maps_Contract_BlockRangeData_To_Api_Model()
        {
            var defaultBlockRangeData = new BlockRangeData
            {
                ContractsCreatedCount = 1,
                FirstBlockNumber = 1,
                LastBlockNumber = 2,
                TransferTotal = 100,
                AddressTransferTotals = new List<AddressTransferTotal>
                {
                    new AddressTransferTotal
                    {
                        Address = testContractAddress,
                        FirstTransactionBlock=1,
                        LastTransactionBlock=2,
                        Received = 65,
                        Sent = 35,
                        TransactionsSeenInCount = 2,
                        IsContract = true
                    }
                }
            };

            var result = _sut.MapBlockRangeDataToReportResponse(defaultBlockRangeData);

            Assert.Equal(1, result.ContractsCreated);
            Assert.Equal(1, result.FirstBlock);
            Assert.Equal(2, result.LastBlock);
            Assert.Equal(100, result.TotalTransfered);
            Assert.NotEmpty(result.AddressTransferTotals);

            Assert.True(result.AddressTransferTotals.ContainsKey(true));
            Assert.False(result.AddressTransferTotals.ContainsKey(false));

            var contractResultCount = result.AddressTransferTotals.Count;

            Assert.Equal(1, contractResultCount);

            var firstContractResult = result.AddressTransferTotals[true].First();

            Assert.Equal(testContractAddress, firstContractResult.Address);
            Assert.Equal(1, firstContractResult.FirstBlockSeenInRange);
            Assert.Equal(2, firstContractResult.LatestBlockSeenInRange);
            Assert.True(firstContractResult.IsContractAddress);
            Assert.Equal(65, firstContractResult.Received);
            Assert.Equal(35, firstContractResult.Sent);
        }

        [Fact]
        public void Mapper_Maps_Externally_OwnedAccount_BlockRangeData_To_Api_Model()
        {
            var defaultBlockRangeData = new BlockRangeData
            {
                ContractsCreatedCount = 1,
                FirstBlockNumber = 1,
                LastBlockNumber = 2,
                TransferTotal = 100,
                AddressTransferTotals = new List<AddressTransferTotal>
                {
                    new AddressTransferTotal
                    {
                        Address = testExternallyOwnedAccountAddress,
                        FirstTransactionBlock=1,
                        LastTransactionBlock=2,
                        Received = 65,
                        Sent = 35,
                        TransactionsSeenInCount = 2,
                        IsContract = false
                    }
                }
            };

            var result = _sut.MapBlockRangeDataToReportResponse(defaultBlockRangeData);

            Assert.Equal(1, result.ContractsCreated);
            Assert.Equal(1, result.FirstBlock);
            Assert.Equal(2, result.LastBlock);
            Assert.Equal(100, result.TotalTransfered);
            Assert.NotEmpty(result.AddressTransferTotals);

            Assert.False(result.AddressTransferTotals.ContainsKey(true));
            Assert.True(result.AddressTransferTotals.ContainsKey(false));

            var resultCount = result.AddressTransferTotals.Count;

            Assert.Equal(1, resultCount);

            var firstExternallyOwnedAccountTotal = result.AddressTransferTotals[false].First();

            Assert.Equal(testExternallyOwnedAccountAddress, firstExternallyOwnedAccountTotal.Address);
            Assert.Equal(1, firstExternallyOwnedAccountTotal.FirstBlockSeenInRange);
            Assert.Equal(2, firstExternallyOwnedAccountTotal.LatestBlockSeenInRange);
            Assert.False(firstExternallyOwnedAccountTotal.IsContractAddress);
            Assert.Equal(65, firstExternallyOwnedAccountTotal.Received);
            Assert.Equal(35, firstExternallyOwnedAccountTotal.Sent);
        }

        [Fact]
        public void Mapper_Maps_Externally_OwnedAccount_And_Contract_BlockRangeData_To_Api_Model()
        {
            var defaultBlockRangeData = new BlockRangeData
            {
                ContractsCreatedCount = 1,
                FirstBlockNumber = 1,
                LastBlockNumber = 2,
                TransferTotal = 200,
                AddressTransferTotals = new List<AddressTransferTotal>
                {
                    new AddressTransferTotal
                    {
                        Address = testExternallyOwnedAccountAddress,
                        FirstTransactionBlock=1,
                        LastTransactionBlock=2,
                        Received = 65,
                        Sent = 35,
                        TransactionsSeenInCount = 2,
                        IsContract = false
                    },
                    new AddressTransferTotal
                    {
                        Address = testContractAddress,
                        FirstTransactionBlock=1,
                        LastTransactionBlock=2,
                        Received = 65,
                        Sent = 35,
                        TransactionsSeenInCount = 2,
                        IsContract = true
                    }
                }
            };

            var result = _sut.MapBlockRangeDataToReportResponse(defaultBlockRangeData);

            Assert.Equal(1, result.ContractsCreated);
            Assert.Equal(1, result.FirstBlock);
            Assert.Equal(2, result.LastBlock);
            Assert.Equal(200, result.TotalTransfered);
            Assert.NotEmpty(result.AddressTransferTotals);

            Assert.True(result.AddressTransferTotals.ContainsKey(true));
            Assert.True(result.AddressTransferTotals.ContainsKey(false));

            var addressTransferTotalsCount = result.AddressTransferTotals.Count;

            Assert.Equal(2, addressTransferTotalsCount);

            var firstExternallyOwnedAccountResult = result.AddressTransferTotals[false].First();

            Assert.Equal(testExternallyOwnedAccountAddress, firstExternallyOwnedAccountResult.Address);
            Assert.Equal(1, firstExternallyOwnedAccountResult.FirstBlockSeenInRange);
            Assert.Equal(2, firstExternallyOwnedAccountResult.LatestBlockSeenInRange);
            Assert.False(firstExternallyOwnedAccountResult.IsContractAddress);
            Assert.Equal(65, firstExternallyOwnedAccountResult.Received);
            Assert.Equal(35, firstExternallyOwnedAccountResult.Sent);

            var firstContractAccountResult = result.AddressTransferTotals[true].First();

            Assert.Equal(testContractAddress, firstContractAccountResult.Address);
            Assert.Equal(1, firstContractAccountResult.FirstBlockSeenInRange);
            Assert.Equal(2, firstContractAccountResult.LatestBlockSeenInRange);
            Assert.True(firstContractAccountResult.IsContractAddress);
            Assert.Equal(65, firstContractAccountResult.Received);
            Assert.Equal(35, firstContractAccountResult.Sent);
        }
    }
}
