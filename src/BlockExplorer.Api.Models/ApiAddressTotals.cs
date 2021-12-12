namespace BlockExplorer.Api.Models
{
    public class ApiAddressTotals
    {
        public decimal Received { get; set; }
        public decimal Sent { get; set; }
        public string Address { get; set; }
        public long FirstBlockSeenInRange { get; set; }
        public long LatestBlockSeenInRange { get; set; }
        public bool IsContractAddress { get; set; }
        public long TransactionsSeenInCount { get; set; }
    }
}
