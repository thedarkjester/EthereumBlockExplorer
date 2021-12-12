using System;
using System.Collections.Generic;

namespace BlockExplorer.Domain.Models
{
    public class BlockRangeData
    {
        public long ContractsCreatedCount { get; set; }
        public long FirstBlockNumber { get; set; } // do I need this?
        public long LastBlockNumber { get; set; } // do I need this?
        public decimal TransferTotal { get; set; }
        public IEnumerable<AddressTransferTotal> AddressTransferTotals { get; set; }
    }
}