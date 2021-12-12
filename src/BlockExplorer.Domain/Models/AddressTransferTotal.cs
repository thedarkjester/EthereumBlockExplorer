using System.Numerics;

namespace BlockExplorer.Domain.Models
{
    public class AddressTransferTotal : EthereumAddress
    {
        public long FirstTransactionBlock { get; set; }
        public long LastTransactionBlock { get; set; }
        public long TransactionsSeenInCount { get; set; }
        public decimal Sent { get; set; }
        public decimal Received { get; set; }
    }
}