using System.Collections.Generic;

namespace BlockExplorer.Api.Models
{
    public class ApiTransferReportResponse
    {
        public decimal TotalTransfered { get; set; }
        public long FirstBlock { get; set; }
        public long LastBlock { get; set; }
        public long ContractsCreated { get; set; }
        public Dictionary<bool, IEnumerable<ApiAddressTotals>> AddressTransferTotals { get; set; }
    }
}
