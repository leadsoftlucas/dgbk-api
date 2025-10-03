using LeadSoft.Common.Library.Constants;
using LucasRT.DGBK.RestApi.Application.Contracts.Payments;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Payments;
using LucasRT.DGBK.RestApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController(IPaymentService payments, ILogger<PaymentsController> logger) : ControllerBase
    {
        [SwaggerOperation(Summary = "", Description = "")]
        [HttpPost("", Name = nameof(PostPaymentAsync))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DtoPaymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DtoPaymentResponse), StatusCodes.Status201Created)]
        [Produces(Constant.ApplicationProblemJson)]
        [Consumes(Constant.ApplicationJson)]
        [Idempotent]
        public async Task<ActionResult<DtoPaymentResponse>> PostPaymentAsync([FromHeader][Required] Guid IdempotencyKey, [FromBody] DtoPaymentRequest dtoRequest)
        {
            DtoPaymentResponse dto = await payments.CreatePaymentAsync(dtoRequest.SetTransactionId(IdempotencyKey));
            return CreatedAtAction(nameof(GetPaymentAsync), new { id = dto.Id }, dto);
        }

        [SwaggerOperation(Summary = "", Description = "")]
        [HttpGet("{id:guid}", Name = nameof(GetPaymentAsync))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DtoPaymentResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        public async Task<ActionResult<DtoPaymentResponse>> GetPaymentAsync([FromRoute] Guid id)
        {
            DtoPaymentResponse dto = await payments.GetPaymentAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [SwaggerOperation(Summary = "", Description = "")]
        [HttpGet("", Name = nameof(GetPaymentsAsync))]
        [ProducesResponseType(typeof(DtoPaymentResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        public async Task<ActionResult<IList<DtoPaymentResponse>>> GetPaymentsAsync([FromQuery] PaymentStatus? paymentStatus = null)
            => Ok(await payments.GetPaymentsAsync(paymentStatus));
    }
}
