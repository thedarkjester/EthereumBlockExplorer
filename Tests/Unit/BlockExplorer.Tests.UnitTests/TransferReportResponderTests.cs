using BlockExplorer.Api.Models;
using BlockExplorer.Domain.Clients;
using BlockExplorer.Domain.Models;
using BlockExplorer.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlockExplorer.Tests.UnitTests
{
    public class TransferReportResponderTests : TestingBase
    {
        private readonly TransferReportResponder _sut;
        private readonly Mock<ILogger<TransferReportResponder>> _mockLogger;
        private readonly Mock<IBlockRangeDataMapper> _mockBlockRangeDataMapper;
        private readonly Mock<IBlockChainClient> _mockBlockChainClient;

        public TransferReportResponderTests()
        {
            _mockLogger = new Mock<ILogger<TransferReportResponder>>();
            _mockBlockRangeDataMapper = new Mock<IBlockRangeDataMapper>();
            _mockBlockChainClient = new Mock<IBlockChainClient>();

            _sut = new TransferReportResponder(_mockLogger.Object, _mockBlockChainClient.Object, _mockBlockRangeDataMapper.Object);
        }

        [Fact]
        public async Task RetrieveRangedBlockData_Returns_Only_Contract_Mapped_Response_Data()
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

            _mockBlockChainClient.Setup(x => x.GetBlockRangeData(1, 2)).ReturnsAsync(defaultBlockRangeData).Verifiable();

            // ideally set up some drivers/builds to build test cases
            _mockBlockRangeDataMapper.Setup(x => x.MapBlockRangeDataToReportResponse(It.Is<BlockRangeData>(y => y.Equals(defaultBlockRangeData)))).Returns(
                new ApiTransferReportResponse
                {
                    ContractsCreated = 1,
                    FirstBlock = 1,
                    LastBlock = 2,
                    TotalTransfered = 100,
                    AddressTransferTotals = new Dictionary<bool, IEnumerable<ApiAddressTotals>>
                    {
                        [true] = BuildContractOwnedAddressTransferTotals()
                    }
                }
                );

            var result = await _sut.RetrieveRangedBlockData(1, 2);

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

            _mockBlockChainClient.Verify();
            _mockBlockRangeDataMapper.Verify();
        }

        [Fact]
        public async Task RetrieveRangedBlockData_Returns_Only_ExternallyOwnedAccount_Mapped_Response_Data()
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

            _mockBlockChainClient.Setup(x => x.GetBlockRangeData(1, 2)).ReturnsAsync(defaultBlockRangeData).Verifiable();

            // ideally set up some drivers/builds to build test cases
            _mockBlockRangeDataMapper.Setup(x => x.MapBlockRangeDataToReportResponse(It.Is<BlockRangeData>(y => y.Equals(defaultBlockRangeData)))).Returns(
                new ApiTransferReportResponse
                {
                    ContractsCreated = 1,
                    FirstBlock = 1,
                    LastBlock = 2,
                    TotalTransfered = 100,
                    AddressTransferTotals = new Dictionary<bool, IEnumerable<ApiAddressTotals>>
                    {
                        [false] = BuildExternallyOwnedAddressTransferTotals()
                    }
                }
                );

            var result = await _sut.RetrieveRangedBlockData(1, 2);

            Assert.Equal(1, result.ContractsCreated);
            Assert.Equal(1, result.FirstBlock);
            Assert.Equal(2, result.LastBlock);
            Assert.Equal(100, result.TotalTransfered);
            Assert.NotEmpty(result.AddressTransferTotals);

            Assert.False(result.AddressTransferTotals.ContainsKey(true));
            Assert.True(result.AddressTransferTotals.ContainsKey(false));

            var externallyOwnedAccountCount = result.AddressTransferTotals.Count;

            Assert.Equal(1, externallyOwnedAccountCount);

            var firstExternallyOwnedAccountResult = result.AddressTransferTotals[false].First();

            Assert.Equal(testExternallyOwnedAccountAddress, firstExternallyOwnedAccountResult.Address);
            Assert.Equal(1, firstExternallyOwnedAccountResult.FirstBlockSeenInRange);
            Assert.Equal(2, firstExternallyOwnedAccountResult.LatestBlockSeenInRange);
            Assert.False(firstExternallyOwnedAccountResult.IsContractAddress);
            Assert.Equal(65, firstExternallyOwnedAccountResult.Received);
            Assert.Equal(35, firstExternallyOwnedAccountResult.Sent);

            _mockBlockChainClient.Verify();
            _mockBlockRangeDataMapper.Verify();
        }

        [Fact]
        public async Task RetrieveRangedBlockData_Returns_Only_ExternallyOwnedAccount_And_Contract_Mapped_Response_Data()
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

            _mockBlockChainClient.Setup(x => x.GetBlockRangeData(1, 2)).ReturnsAsync(defaultBlockRangeData).Verifiable();

            // ideally set up some drivers/builds to build test cases
            _mockBlockRangeDataMapper.Setup(x => x.MapBlockRangeDataToReportResponse(It.Is<BlockRangeData>(y => y.Equals(defaultBlockRangeData)))).Returns(
                new ApiTransferReportResponse
                {
                    ContractsCreated = 1,
                    FirstBlock = 1,
                    LastBlock = 2,
                    TotalTransfered = 200,
                    AddressTransferTotals = new Dictionary<bool, IEnumerable<ApiAddressTotals>>
                    {
                        [false] = BuildExternallyOwnedAddressTransferTotals(),
                        [true] = BuildContractOwnedAddressTransferTotals()
                    }
                }
                );

            var result = await _sut.RetrieveRangedBlockData(1, 2);

            Assert.Equal(1, result.ContractsCreated);
            Assert.Equal(1, result.FirstBlock);
            Assert.Equal(2, result.LastBlock);
            Assert.Equal(200, result.TotalTransfered);
            Assert.NotEmpty(result.AddressTransferTotals);

            Assert.True(result.AddressTransferTotals.ContainsKey(true));
            Assert.True(result.AddressTransferTotals.ContainsKey(false));

            var contractResultCount = result.AddressTransferTotals.Count;
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

            _mockBlockChainClient.Verify();
            _mockBlockRangeDataMapper.Verify();
        }

        [Fact]
        public async Task RetrieveCurrentAndPreceedingBlocks_Returns_Only_Contract_Mapped_Response_Data()
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

            _mockBlockChainClient.Setup(x => x.GetRecentBlockRangeData(1)).ReturnsAsync(defaultBlockRangeData).Verifiable();

            // ideally set up some drivers/builds to build test cases
            _mockBlockRangeDataMapper.Setup(x => x.MapBlockRangeDataToReportResponse(It.Is<BlockRangeData>(y => y.Equals(defaultBlockRangeData)))).Returns(
                new ApiTransferReportResponse
                {
                    ContractsCreated = 1,
                    FirstBlock = 1,
                    LastBlock = 2,
                    TotalTransfered = 100,
                    AddressTransferTotals = new Dictionary<bool, IEnumerable<ApiAddressTotals>>
                    {
                        [true] = BuildContractOwnedAddressTransferTotals()
                    }
                }
                );

            var result = await _sut.RetrieveCurrentAndPreceedingBlocksData(1);

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

            _mockBlockChainClient.Verify();
            _mockBlockRangeDataMapper.Verify();
        }

        [Fact]
        public async Task RetrieveCurrentAndPreceedingBlocks_Returns_Only_ExternallyOwnedAccount_Mapped_Response_Data()
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

            _mockBlockChainClient.Setup(x => x.GetRecentBlockRangeData(1)).ReturnsAsync(defaultBlockRangeData).Verifiable();

            // ideally set up some drivers/builds to build test cases
            _mockBlockRangeDataMapper.Setup(x => x.MapBlockRangeDataToReportResponse(It.Is<BlockRangeData>(y => y.Equals(defaultBlockRangeData)))).Returns(
                new ApiTransferReportResponse
                {
                    ContractsCreated = 1,
                    FirstBlock = 1,
                    LastBlock = 2,
                    TotalTransfered = 100,
                    AddressTransferTotals = new Dictionary<bool, IEnumerable<ApiAddressTotals>>
                    {
                        [false] = BuildExternallyOwnedAddressTransferTotals()
                    }
                }
                );

            var result = await _sut.RetrieveCurrentAndPreceedingBlocksData(1);

            Assert.Equal(1, result.ContractsCreated);
            Assert.Equal(1, result.FirstBlock);
            Assert.Equal(2, result.LastBlock);
            Assert.Equal(100, result.TotalTransfered);
            Assert.NotEmpty(result.AddressTransferTotals);

            Assert.False(result.AddressTransferTotals.ContainsKey(true));
            Assert.True(result.AddressTransferTotals.ContainsKey(false));

            var externallyOwnedAccountCount = result.AddressTransferTotals.Count;

            Assert.Equal(1, externallyOwnedAccountCount);

            var firstExternallyOwnedAccountResult = result.AddressTransferTotals[false].First();

            Assert.Equal(testExternallyOwnedAccountAddress, firstExternallyOwnedAccountResult.Address);
            Assert.Equal(1, firstExternallyOwnedAccountResult.FirstBlockSeenInRange);
            Assert.Equal(2, firstExternallyOwnedAccountResult.LatestBlockSeenInRange);
            Assert.False(firstExternallyOwnedAccountResult.IsContractAddress);
            Assert.Equal(65, firstExternallyOwnedAccountResult.Received);
            Assert.Equal(35, firstExternallyOwnedAccountResult.Sent);

            _mockBlockChainClient.Verify();
            _mockBlockRangeDataMapper.Verify();
        }

        [Fact]
        public async Task RetrieveCurrentAndPreceedingBlocks_Returns_Only_ExternallyOwnedAccount_And_Contract_Mapped_Response_Data()
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

            _mockBlockChainClient.Setup(x => x.GetRecentBlockRangeData(1)).ReturnsAsync(defaultBlockRangeData).Verifiable();

            // ideally set up some drivers/builds to build test cases
            _mockBlockRangeDataMapper.Setup(x => x.MapBlockRangeDataToReportResponse(It.Is<BlockRangeData>(y => y.Equals(defaultBlockRangeData)))).Returns(
                new ApiTransferReportResponse
                {
                    ContractsCreated = 1,
                    FirstBlock = 1,
                    LastBlock = 2,
                    TotalTransfered = 200,
                    AddressTransferTotals = new Dictionary<bool, IEnumerable<ApiAddressTotals>>
                    {
                        [false] = BuildExternallyOwnedAddressTransferTotals(),
                        [true] = BuildContractOwnedAddressTransferTotals()
                    }
                }
                );

            var result = await _sut.RetrieveCurrentAndPreceedingBlocksData(1);

            Assert.Equal(1, result.ContractsCreated);
            Assert.Equal(1, result.FirstBlock);
            Assert.Equal(2, result.LastBlock);
            Assert.Equal(200, result.TotalTransfered);
            Assert.NotEmpty(result.AddressTransferTotals);

            Assert.True(result.AddressTransferTotals.ContainsKey(true));
            Assert.True(result.AddressTransferTotals.ContainsKey(false));

            var contractResultCount = result.AddressTransferTotals.Count;
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

            _mockBlockChainClient.Verify();
            _mockBlockRangeDataMapper.Verify();
        }
    }
}
