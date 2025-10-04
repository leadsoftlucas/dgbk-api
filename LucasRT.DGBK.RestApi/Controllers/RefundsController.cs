using LeadSoft.Common.Library.Constants;
using LucasRT.DGBK.RestApi.Application.Contracts.Refunds;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Refunds;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Controllers
{
    /// <summary>
    /// Provides endpoints for managing refund operations, including creating, retrieving, and listing refunds.
    /// </summary>
    /// <remarks>The <see cref="RefundsController"/> class is responsible for handling HTTP requests related
    /// to refund operations. It allows clients to initiate refund requests, retrieve specific refund details by ID, and
    /// list refunds with optional filtering by status.</remarks>
    /// <param name="refunds"></param>
    /// <param name="logger"></param>
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Controller de Reembolsos | Controller responsável por gerar, obter e pesquisar por ordens de reembolsos em pagamentos.")]
    public class RefundsController(IRefundService refunds, ILogger<RefundsController> logger) : ControllerBase
    {
        /// <summary>
        /// Initiates a refund request for a specified payment.
        /// </summary>
        /// <remarks>This method processes a refund request by creating a refund order for the specified
        /// payment.  It returns a 200 OK status with the refund details if successful, or an appropriate error status
        /// if the request cannot be processed.</remarks>
        /// <param name="dtoRequest">The refund request details encapsulated in a <see cref="DtoRefundRequest"/> object. This parameter cannot be
        /// null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="DtoRefundResponse"/> object with the details of the processed refund request.</returns>
        [SwaggerOperation(Summary = "Método para inserir uma ordem de reembolso", Description = "Este método irá inserir para um pagamento, um pedido de reembolso a ser processado.")]
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

        /// <summary>
        /// Retrieves the refund details for the specified refund identifier.
        /// </summary>
        /// <remarks>This method returns a 200 OK response with the refund details if the refund is found,
        /// a 404 Not Found response if the refund does not exist, or a 400 Bad Request response if the input is
        /// invalid.</remarks>
        /// <param name="id">The unique identifier of the refund to retrieve. Must be a valid GUID.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing a <see cref="DtoRefundResponse"/> if the refund is found;
        /// otherwise, a <see cref="NotFoundResult"/> if no refund exists for the specified identifier.</returns>
        [SwaggerOperation(Summary = "Método para obter um reembolso por Id", Description = "Obtém o reembolso para o Id especificado.")]
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

        /// <summary>
        /// Retrieves a list of refunds, optionally filtered by refund status.
        /// </summary>
        /// <remarks>This method supports querying refunds based on their status, allowing clients to
        /// retrieve only those refunds that match the specified criteria. If no status is provided, the method returns
        /// all available refunds.</remarks>
        /// <param name="refundStatus">An optional parameter to filter the refunds by their status. If <see langword="null"/>, all refunds are
        /// returned.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ActionResult{T}"/>
        /// with a list of <see cref="DtoRefundResponse"/> objects representing the refunds.</returns>
        [SwaggerOperation(Summary = "Método para listar e pesquisar reembolsos", Description = "Método disponível para filtrar ou listar os reembolsos existentes na base de dados")]
        [HttpGet("", Name = nameof(GetRefundsAsync))]
        [ProducesResponseType(typeof(DtoRefundResponse), StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        public async Task<ActionResult<IList<DtoRefundResponse>>> GetRefundsAsync([FromQuery] RefundStatus? refundStatus = null)
            => Ok(await refunds.GetRefundsAsync(refundStatus));
    }
}
