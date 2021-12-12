using BlockExplorer.Api.Models;
using System.Collections.Generic;

namespace BlockExplorer.Tests.UnitTests
{
    public class TestingBase
    {
        protected readonly string testContractAddress = "0x45a36a8e118c37e4c47ef4ab827a7c9e579e11e2";
        protected readonly string testExternallyOwnedAccountAddress = "0x3aB28eCeDEa6cdb6feeD398E93Ae8c7b316B1182";

        protected List<ApiAddressTotals> BuildAddressTransferTotals(string address, bool isContract)
        {
            return new List<ApiAddressTotals>
                        {
                            new ApiAddressTotals
                            {
                                Address = address,
                                FirstBlockSeenInRange=1,
                                LatestBlockSeenInRange=2,
                                Received = 65,
                                Sent = 35,
                                TransactionsSeenInCount = 2,
                                IsContractAddress = isContract
                            }
                        };
        }

        protected List<ApiAddressTotals> BuildExternallyOwnedAddressTransferTotals()
        {
            return BuildAddressTransferTotals(testExternallyOwnedAccountAddress, false);
        }

        protected List<ApiAddressTotals> BuildContractOwnedAddressTransferTotals()
        {
            return BuildAddressTransferTotals(testContractAddress, true);
        }
    }
}
