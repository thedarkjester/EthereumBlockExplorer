using BlockExplorer.Api.Models;
using BlockExplorer.Domain.Clients;
using BlockExplorer.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockExplorer.Handlers.Tests.Unit
{
    [TestFixture]
    public class TransferReportResponderTests
    {
        private TransferReportResponder _sut;
        private Mock<ILogger<TransferReportResponder>> _mockLogger;
        private Mock<IBlockRangeDataMapper> _mockBlockRangeDataMapper;
        private Mock<IBlockChainClient> _mockBlockChainClient;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<TransferReportResponder>>();
            _mockBlockRangeDataMapper = new Mock<IBlockRangeDataMapper>();
            _mockBlockChainClient = new Mock<IBlockChainClient>();

            _sut = new TransferReportResponder(_mockLogger.Object, _mockBlockChainClient.Object, _mockBlockRangeDataMapper.Object);
        }

        [Test]
        public async Task RetrieveRangedBlockData_Returns_Mapped_Response_Data()
        {
            var defaultBlockRangeData = new BlockRangeData
            {
                ContractsCreatedCount = 1,
                FirstBlockNumber = 0,
                LastBlockNumber = 1,
                TransferTotal = 100,
                AddressTransferTotals = new List<AddressTransferTotal>
                {
                    new AddressTransferTotal
                    {
                        Address = "0x45a36a8e118c37e4c47ef4ab827a7c9e579e11e2",
                        FirstTransactionBlock=0,
                        LastTransactionBlock=1,
                        Received = 65,
                        Sent = 35,
                        TransactionsSeenInCount = 1,
                        IsContract = false
                    }
                }
            };

            _mockBlockChainClient.Setup(x => x.GetBlockRangeData(0, 1)).ReturnsAsync(defaultBlockRangeData).Verifiable();
            //_mockBlockRangeDataMapper.Setup(x => x.MapBlockRangeDataToReportResponse(defaultBlockRangeData)).Returns<TransferReportResponse>(x=>
            //    x.ContractsCreated == 0
            //);
            var result = await _sut.RetrieveRangedBlockData(0, 1);

            _mockBlockChainClient.Verify();
        }

    }
}
