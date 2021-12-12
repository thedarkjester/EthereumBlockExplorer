using BlockExplorer.Domain.Clients;
using BlockExplorer.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using System.Collections.Generic;
using Nethereum.RPC.Eth.DTOs;

namespace BlockExplorer.Clients
{
    public class NethereumClient : IBlockChainClient
    {
        private readonly ILogger<NethereumClient> _logger;
        private readonly IWeb3Provider _web3Provider;

        public NethereumClient(ILogger<NethereumClient> logger, IWeb3Provider web3Provider)
        {
            _logger = logger;
            _web3Provider = web3Provider;
        }

        public async Task<BlockRangeData> GetBlockRangeData(long firstBlock, long lastBlock)
        {
            var web3 = _web3Provider.GetWeb3();

            var highestBlockToUse = lastBlock;

            if (highestBlockToUse < firstBlock)
            {
                _logger.LogWarning($"Requested highest block {highestBlockToUse} is larger than first block {firstBlock}");

                highestBlockToUse = firstBlock;
            }

            var latestBlock = await GetLatestBlockNumber(web3);

            if (latestBlock < highestBlockToUse)
            {
                _logger.LogWarning($"Requested highest block {highestBlockToUse} is larger than block height {latestBlock}");

                highestBlockToUse = latestBlock;
            }

            var tempData = new Dictionary<string, AddressTransferTotal>();
            decimal totalSent = 0M;
            long contractsCreated = 0;

            // this loop could ideally be done with a async whenAll and a polly retry for each using a concurrent thread-safe implementation
            for (long blockNumber = firstBlock; blockNumber <= highestBlockToUse; blockNumber++)
            {
                var blockWithTransactions = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockNumber));

                foreach (var transactions in blockWithTransactions.Transactions)
                {
                    var totalsAndContractsCreated = await AggregateTransactionData(tempData, transactions, blockNumber, web3);
                    totalSent += totalsAndContractsCreated.totalSent;
                    contractsCreated += totalsAndContractsCreated.contractsCreated;
                }
            }

            var blockRangeData = new BlockRangeData
            {
                FirstBlockNumber = firstBlock,
                LastBlockNumber = highestBlockToUse,
                AddressTransferTotals = tempData.Values,
                TransferTotal = totalSent,
                ContractsCreatedCount = contractsCreated
            };

            return blockRangeData;
        }

        private async Task<long> GetLatestBlockNumber(IWeb3 web3)
        {
            var blockHeight = (long)(await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).ToUlong();

            _logger.LogInformation($"GetLatestBlockNumber retrieved Block height={blockHeight}");

            return blockHeight;
        }

        public async Task<BlockRangeData> GetRecentBlockRangeData(long howManyRecentBlocks)
        {
            var web3 = _web3Provider.GetWeb3();

            var latestBlock = await GetLatestBlockNumber(web3);
            var startingBlock = latestBlock - howManyRecentBlocks;

            if (startingBlock < 0)
            {
                _logger.LogWarning($"Requested howManyRecentBlocks {howManyRecentBlocks} produced negative start - using startingBlock=0");

                startingBlock = 0;
            }

            var rangeData = await GetBlockRangeData(startingBlock, latestBlock);

            return rangeData;
        }

        private async Task<(decimal totalSent, long contractsCreated)> AggregateTransactionData(Dictionary<string, AddressTransferTotal> tempData, Transaction transaction, long blockNumber, IWeb3 web3)
        {
            long contractsCreated = 0;
            decimal totalSent = 0;

            var transactionValue = Web3.Convert.FromWei(transaction.Value);
            totalSent += transactionValue;

            bool isContract;

            //TODO refactor for sent
            if (!tempData.TryGetValue(transaction.From, out AddressTransferTotal sentAddressTotal))
            {
                isContract = await LookupContract(transaction.From, web3);

                sentAddressTotal = new AddressTransferTotal
                {
                    Address = transaction.From,
                    FirstTransactionBlock = blockNumber,
                    LastTransactionBlock = blockNumber,
                    Sent = transactionValue,
                    IsContract = isContract
                };

                tempData.Add(transaction.From, sentAddressTotal);
            }

            sentAddressTotal.TransactionsSeenInCount++;
            sentAddressTotal.Sent += transactionValue;
            sentAddressTotal.LastTransactionBlock = blockNumber;

            //TODO refactor for received
            if (transaction.To == null)
            {
                contractsCreated++;

                // need to find a way to add the address of the contract to the collection as technically it exists at the block
            }
            else
            {
                if (!tempData.TryGetValue(transaction.To, out AddressTransferTotal receivedAddressTotal))
                {
                    isContract = await LookupContract(transaction.To, web3);

                    receivedAddressTotal = new AddressTransferTotal
                    {
                        Address = transaction.To,
                        FirstTransactionBlock = blockNumber,
                        LastTransactionBlock = blockNumber,
                        Received = transactionValue,
                        IsContract = isContract
                    };

                    tempData.Add(transaction.To, receivedAddressTotal);
                }

                receivedAddressTotal.TransactionsSeenInCount++;
                receivedAddressTotal.Received += transactionValue;
                receivedAddressTotal.LastTransactionBlock = blockNumber;
            }

            return (totalSent, contractsCreated);
        }

        private async Task<bool> LookupContract(string address, IWeb3 web3)
        {
            var codeAtAddress = await web3.Eth.GetCode.SendRequestAsync(address);
            if (codeAtAddress != "0x")
            {
                return true;
            }

            return false;
        }
    }
}
