using BlockExplorer.Api.Models;
using BlockExplorer.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace BlockExplorer.Api.Controllers
{
    [ApiController]
    [Route("TransferReport")]
    public class TransferReportController : ControllerBase
    {
        private readonly IAnswerTransferReportResponse _transferReportResponder;

        public TransferReportController(IAnswerTransferReportResponse transferReportResponder)
        {
            _transferReportResponder = transferReportResponder;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiTransferReportResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiTransferReportResponse), (int)HttpStatusCode.BadRequest)]
        [Route("ByRange")]
        public async Task<ActionResult<ApiTransferReportResponse>> RetrieveRangedBlockData(
            [Range(0, int.MaxValue)][FromQuery] long startBlock,
            [FromQuery] long lastBlock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid, please use positive numbers.");
            }

            var response = await _transferReportResponder.RetrieveRangedBlockData(startBlock, lastBlock);

            return response;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiTransferReportResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiTransferReportResponse), (int)HttpStatusCode.BadRequest)]
        [Route("ByRecentAmount")]
        public async Task<ActionResult<ApiTransferReportResponse>> RetrieveCurrentAndPreceedingBlocks([Range(0, int.MaxValue)][FromQuery] long preceedingBlocks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid, please use positive numbers.");
            }

            var response = await _transferReportResponder.RetrieveCurrentAndPreceedingBlocksData(preceedingBlocks);

            return response;
        }
    }
}
