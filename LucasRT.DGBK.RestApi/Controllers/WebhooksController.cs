using LeadSoft.Common.Library.Constants;
using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Application.Contracts.Webhooks;
using LucasRT.DGBK.RestApi.Infrastructure;
using LucasRT.DGBK.RestApi.Workers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LucasRT.DGBK.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController(ILogger<WebhooksController> logger) : ControllerBase
    {
        [SwaggerOperation(Summary = "", Description = "")]
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

        [SwaggerOperation(Summary = "", Description = "")]
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
    }
}
