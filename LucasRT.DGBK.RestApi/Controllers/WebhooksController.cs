using LeadSoft.Common.Library.Constants;
using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Application.Contracts.Webhooks;
using LucasRT.DGBK.RestApi.Infrastructure;
using LucasRT.DGBK.RestApi.Workers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LucasRT.DGBK.RestApi.Controllers
{

    /// <summary>
    /// Provides endpoints for simulating webhook interactions, including payment and refund processing.
    /// </summary>
    /// <remarks>This controller is designed to simulate external API interactions for testing purposes. It
    /// includes endpoints for processing payment and refund requests, validating HMAC signatures, and generating random
    /// outcomes to mimic real-world scenarios. The controller requires specific headers for authentication and
    /// processes requests based on the provided data transfer objects.</remarks>
    /// <param name="logger"></param>
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Controller de Webhook | Endpoints fake de webhook para simular uma API externa e validar o HMAC da requisição.")]
    public class WebhooksController(ILogger<WebhooksController> logger) : ControllerBase
    {
        /// <summary>
        /// Processes a payment request by validating the HMAC signature and simulating a payment response.
        /// </summary>
        /// <remarks>This method simulates the processing of a payment by generating a random outcome.  It
        /// requires a valid HMAC signature and timestamp in the request headers to proceed.</remarks>
        /// <param name="dtoRequest">The payment request data transfer object containing payment details.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the payment processing.  Returns <see
        /// cref="StatusCodes.Status200OK"/> for a successful payment,  <see cref="StatusCodes.Status400BadRequest"/>
        /// for a malformed request,  <see cref="StatusCodes.Status411LengthRequired"/> if required headers are missing,
        /// <see cref="StatusCodes.Status422UnprocessableEntity"/> for an invalid signature,  or <see
        /// cref="StatusCodes.Status500InternalServerError"/> for an unexpected error.</returns>
        [SwaggerOperation(Summary = "Webhook de payments", Description = "Gera uma resposta fake de processamento de pagamento e validação de chave HMAC")]
        [HttpPost("payment", Name = nameof(PostProcessPayment))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status411LengthRequired)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        [Consumes(Constant.ApplicationJson)]
        public IActionResult PostProcessPayment([FromBody] DtoProcessPaymentRequest dtoRequest)
        {
            string signature = Request.GetHeader(RefundWebhookDeliveryWorker.WebhookHeader_Signature);
            if (signature.IsNothing())
                return StatusCode(411, "Missing signature");

            string timestamp = Request.GetHeader(RefundWebhookDeliveryWorker.WebhookHeader_Timestamp);
            if (timestamp.IsNothing())
                return StatusCode(411, "Missing timestamp");

            if (!dtoRequest.DtoPayment.ToJson().VerifyHmac(signature, timestamp))
                return StatusCode(422, "Invalid signature");

            int random = new Random().Next(0, 10);

            logger.LogInformation("Webhook received {Random}", random);

            return random switch
            {
                0 => RejectedPayment,
                1 => SucessfulPayment,
                2 => SucessfulPayment,
                3 => TimeItOut,
                4 => SucessfulPayment,
                5 => StatusCode(500, $"Webhook received random: {random}"),
                6 => SucessfulPayment,
                7 => SucessfulPayment,
                8 => SucessfulPayment,
                9 => RejectedPayment,
                10 => SucessfulPayment,
                _ => StatusCode(500)
            };
        }

        /// <summary>
        /// Processes a refund request by validating the HMAC signature and simulating a refund response.
        /// </summary>
        /// <remarks>This method simulates the processing of a refund request by generating a random
        /// outcome.  It requires a valid HMAC signature and timestamp in the request headers to proceed.</remarks>
        /// <param name="dtoRequest">The refund request data transfer object containing the refund details to be processed.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the refund processing.  Returns a 200 OK status
        /// for a successful refund, 400 Bad Request for invalid input,  411 Length Required if required headers are
        /// missing, 422 Unprocessable Entity for an invalid signature,  or 500 Internal Server Error for unexpected
        /// conditions.</returns>
        [SwaggerOperation(Summary = "Webhook de reembolso", Description = "Gera uma resposta fake de processamento de reembolso e validação de chave HMAC")]
        [HttpPost("refund", Name = nameof(PostProcessRefund))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status411LengthRequired)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(Constant.ApplicationProblemJson)]
        [Consumes(Constant.ApplicationJson)]
        public IActionResult PostProcessRefund([FromBody] DtoProcessRefundRequest dtoRequest)
        {
            string signature = Request.GetHeader(RefundWebhookDeliveryWorker.WebhookHeader_Signature);
            if (signature.IsNothing())
                return StatusCode(411, "Missing signature");

            string timestamp = Request.GetHeader(RefundWebhookDeliveryWorker.WebhookHeader_Timestamp);
            if (timestamp.IsNothing())
                return StatusCode(411, "Missing timestamp");

            if (!dtoRequest.DtoRefund.ToJson().VerifyHmac(signature, timestamp))
                return StatusCode(422, "Invalid signature");

            int random = new Random().Next(0, 5);

            logger.LogInformation("Webhook received {Random}", random);

            return random switch
            {
                0 => RejectedPayment,
                1 => SucessfulPayment,
                2 => SucessfulPayment,
                3 => TimeItOut,
                4 => SucessfulPayment,
                5 => StatusCode(500, $"Webhook received random: {random}"),
                6 => SucessfulPayment,
                7 => RejectedPayment,
                8 => SucessfulPayment,
                9 => SucessfulPayment,
                10 => SucessfulPayment,
                _ => StatusCode(500)
            };
        }

        #region [ Respostas fake ]
        private IActionResult RejectedPayment
        {
            get
            {
                logger.LogInformation("Webhook rejected");
                return BadRequest();
            }
        }

        private IActionResult SucessfulPayment
        {
            get
            {
                logger.LogInformation("Webhook processed successfully");
                return Ok();
            }
        }

        private IActionResult TimeItOut
        {
            get
            {
                Thread.Sleep(60000);
                return StatusCode(408, "Artificial timeout");
            }
        }

        #endregion
    }
}
