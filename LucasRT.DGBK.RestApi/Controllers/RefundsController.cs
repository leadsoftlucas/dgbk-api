using LeadSoft.Common.Library.Constants;
using LucasRT.DGBK.RestApi.Application.Contracts.Refunds;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Refunds;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RefundsController(IRefundService refunds, ILogger<RefundsController> logger) : ControllerBase
    {
        [SwaggerOperation(Summary = "", Description = "")]
        [HttpPost("", Name = nameof(PostRefundAsync))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status411LengthRequired)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DtoRefundResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        [Consumes(Constant.ApplicationJson)]
        public async Task<ActionResult<DtoRefundResponse>> PostRefundAsync([FromBody] DtoRefundRequest dtoRequest)
        {
            DtoRefundResponse dto = await refunds.CreateRefundAsync(dtoRequest);
            return CreatedAtAction(nameof(GetRefundAsync), new { id = dto.Id }, dto);
        }

        [SwaggerOperation(Summary = "", Description = "")]
        [HttpGet("{id:guid}", Name = nameof(GetRefundAsync))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DtoRefundResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        public async Task<ActionResult<DtoRefundResponse>> GetRefundAsync([FromRoute] Guid id)
        {
            DtoRefundResponse dto = await refunds.GetRefundAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [SwaggerOperation(Summary = "", Description = "")]
        [HttpGet("", Name = nameof(GetRefundsAsync))]
        [ProducesResponseType(typeof(DtoRefundResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        public async Task<ActionResult<IList<DtoRefundResponse>>> GetRefundsAsync([FromQuery] RefundStatus? refundStatus = null)
            => Ok(await refunds.GetRefundsAsync(refundStatus));
    }
}
