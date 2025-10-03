using LeadSoft.Common.GlobalDomain.DTOs;
using LeadSoft.Common.Library.Constants;
using LucasRT.DGBK.RestApi.Application.Contracts.Payments;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Payments;
using LucasRT.DGBK.RestApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace LucasRT.DGBK.RestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController(IPaymentService payment, ILogger<PaymentsController> logger) : ControllerBase
    {
        [SwaggerOperation(Summary = "", Description = "")]
        [HttpPost("", Name = nameof(PostPaymentAsync))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status411LengthRequired)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOBoolResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        [Consumes(Constant.ApplicationJson)]
        [Idempotent]
        public async Task<ActionResult<DtoPaymentResponse>> PostPaymentAsync([FromHeader][Required] Guid IdempotencyKey, [FromBody] DtoPaymentRequest dtoRequest)
        {
            DtoPaymentResponse dto = await payment.CreatePaymentAsync(dtoRequest);
            return CreatedAtAction(nameof(GetPaymentAsync), new { id = dto.Id }, dto);
        }

        [SwaggerOperation(Summary = "", Description = "")]
        [HttpGet("payments/{id:guid}", Name = nameof(GetPaymentAsync))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status411LengthRequired)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOBoolResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        public async Task<ActionResult<DtoPaymentResponse>> GetPaymentAsync(Guid id)
        {
            //var p = await handler.Handle(new(id));
            DtoPaymentResponse p = null;
            return p is null ? NotFound() : Ok(/*PaymentResponse.From(p)*/);
        }
    }
}
