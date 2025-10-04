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
    /// <summary>
    /// Provides endpoints for creating, retrieving, and searching payment orders.
    /// </summary>
    /// <remarks>The <see cref="PaymentsController"/> class is responsible for handling HTTP requests related
    /// to payment operations. It supports creating new payment orders in an idempotent manner, retrieving payment
    /// details by identifier, and listing payments with optional status filtering.</remarks>
    /// <param name="payments"></param>
    /// <param name="logger"></param>
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Controller de Pagamentos | Controller responsável por gerar, obter e pesquisar por ordens de pagamento.")]
    public class PaymentsController(IPaymentService payments, ILogger<PaymentsController> logger) : ControllerBase
    {
        /// <summary>
        /// Creates a new payment order in an idempotent manner.
        /// </summary>
        /// <remarks>This method is idempotent, meaning that multiple identical requests will have the
        /// same effect as a single request. It is important to provide a unique <paramref name="IdempotencyKey"/> for
        /// each new payment attempt to avoid duplicate transactions.</remarks>
        /// <param name="IdempotencyKey">A unique identifier for the request to ensure idempotency. This key must be provided in the request header.</param>
        /// <param name="dtoRequest">The payment request details, including transfer data, provided in the request body.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing a <see cref="DtoPaymentResponse"/> object.  Returns a 200 OK
        /// status if the payment already exists, or a 201 Created status if a new payment is successfully created.</returns>
        [SwaggerOperation(Summary = "Método idempotente para criação de ordem de pagamento.", Description = "Insira os dados de transferência para incluir um pagamento.")]
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

        /// <summary>
        /// Retrieves the payment details for a specified payment identifier.
        /// </summary>
        /// <remarks>This method returns a 200 OK response with the payment details if the payment is
        /// found, a 404 Not Found response if the payment does not exist, or a 400 Bad Request response if the request
        /// is malformed.</remarks>
        /// <param name="id">The unique identifier of the payment to retrieve. Must be a valid GUID.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing a <see cref="DtoPaymentResponse"/> if the payment is found;
        /// otherwise, a <see cref="NotFoundResult"/> if the payment does not exist.</returns>
        [SwaggerOperation(Summary = "Obter pagamento por Id", Description = "Obtenha os dados de um pagamento pelo seu Id")]
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

        /// <summary>
        /// Retrieves a list of payments, optionally filtered by payment status.
        /// </summary>
        /// <remarks>This method returns a 200 OK response with the list of payments. If no payments match
        /// the specified status, an empty list is returned.</remarks>
        /// <param name="paymentStatus">An optional filter to specify the status of payments to retrieve. If null, all payments are returned.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ActionResult{T}"/>
        /// with a list of <see cref="DtoPaymentResponse"/> objects representing the payments.</returns>
        [SwaggerOperation(Summary = "Pesquisar e listar pagamentos", Description = "Liste ou pesquise via filtro, os pagamentos existentes na base de dados.")]
        [HttpGet("", Name = nameof(GetPaymentsAsync))]
        [ProducesResponseType(typeof(DtoPaymentResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        public async Task<ActionResult<IList<DtoPaymentResponse>>> GetPaymentsAsync([FromQuery] PaymentStatus? paymentStatus = null)
            => Ok(await payments.GetPaymentsAsync(paymentStatus));
    }
}
