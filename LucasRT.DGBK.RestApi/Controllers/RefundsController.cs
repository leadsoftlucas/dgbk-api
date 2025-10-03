using LeadSoft.Common.GlobalDomain.DTOs;
using LeadSoft.Common.Library.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LucasRT.DGBK.RestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RefundsController(ILogger<RefundsController> logger) : ControllerBase
    {
        [SwaggerOperation(Summary = "", Description = "")]
        [HttpPost("", Name = nameof(PostRefundAsync))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status411LengthRequired)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOBoolResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        [Consumes(Constant.ApplicationJson)]
        public async Task<IActionResult> PostRefundAsync(/*[FromBody] RefundRequest req, [FromServices] RefundHandler handler*/)
        {
            // var r = await handler.Handle(new RefundCommand(req.PaymentId, req.Amount));
            return Ok(/*RefundResponse.From(r)*/);
        }
    }
}
