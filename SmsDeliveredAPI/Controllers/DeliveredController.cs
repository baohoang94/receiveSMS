using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using SmsDeliveredAPI.Models;
using SmsDeliveredAPI.Services;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SmsDeliveredAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveredController : ControllerBase
    {
        private readonly ILogger<DeliveredController> _logger;
        private readonly IMessagesDeliveredService _messagesDeliveredService;
        private readonly ISystemService _systemService;
        private readonly Crypto.IRsaCrypto _rsaCrypto;

        public DeliveredController(IMessagesDeliveredService messagesDeliveredService,
            ISystemService systemService,
            Crypto.IRsaCrypto rsaCrypto,
            ILogger<DeliveredController> logger)
        {
            _messagesDeliveredService = messagesDeliveredService;
            _systemService = systemService;
            _rsaCrypto = rsaCrypto;
            _logger = logger;
        }

        [HttpGet("messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok();
        }

        [HttpPost("messages")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeliveredTransactionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DeliveredTransactionResponse))]
        public IActionResult Messages([FromBody] DeliveredTransactionRequest deliveredTransactionRequest)
        {
            if (deliveredTransactionRequest == null)
                throw new ArgumentNullException(nameof(deliveredTransactionRequest));

            DeliveredTransactionResponse transactionResponse;
            try
            {
                // Kiểm tra các điều kiện
                // Lấy chữ ký từ Body encode Base64 
                byte[] signature = Convert.FromBase64String(deliveredTransactionRequest.Signature);

                // Lấy byteData từ Body encode Base64
                byte[] byteData = Convert.FromBase64String(deliveredTransactionRequest.Payload);

                // Lấy json string từ byteData (Đã thưc hiện getbytes UTF8)
                string jsonData = Encoding.UTF8.GetString(byteData);

                // Lấy public key của SAMI-S kiểm tra
                var cryptoServiceProvider = _systemService.SamiPublicKey;

                if (!_rsaCrypto.VerifyHashData(signature, byteData, cryptoServiceProvider))
                {
                    return BadRequest("Invalid signature!");
                }

                // Validate Deserialize object
                JsonTextReader textReader = new JsonTextReader(new StringReader(jsonData));

                JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(textReader)
                {
                    Schema = _systemService.DeliveredTransactionSchema
                };

                JsonSerializer serializer = new JsonSerializer();

                MessagesDeliveredTransaction deliveredTransaction = serializer.Deserialize<MessagesDeliveredTransaction>(validatingReader);

                _logger.LogInformation($"Transaction Decoded: {JsonConvert.SerializeObject(deliveredTransaction)}");

                if (deliveredTransaction == null)
                    throw new ArgumentException("The data is not in the correct format, reference https://");

                if (deliveredTransaction.MessagesDelivered.Count() >= 200)
                    return BadRequest(new ErrorResponse
                    {
                        ErrorCode = 400,
                        ErrorMessage = "Not allowed to send more than 200  messages delivered per transaction"
                    });

                // Chuan bi cac ban tin luu vao database
                _messagesDeliveredService.PushMessages(deliveredTransaction.MessagesDelivered);

                // Lưu vào trong Database (Repository)
                _messagesDeliveredService.Save();

                transactionResponse = new DeliveredTransactionResponse
                {
                    Status = _messagesDeliveredService.DeliveredReports.Count,
                    Message = "SUCCESS",
                    ResponseTime = DateTime.Now,
                    Data = new DeliveredData
                    {
                        TransactionID = deliveredTransaction.TransactionId,
                        DeliveredReports = _messagesDeliveredService.DeliveredReports
                    }
                };
                return Ok(transactionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("DeliveredMessages", ex.Message);

                transactionResponse = new DeliveredTransactionResponse
                {
                    Status = -1,
                    Message = ex.Message,
                    ResponseTime = DateTime.Now,
                    Data = new DeliveredData
                    {
                        TransactionID = null,
                        DeliveredReports = null,
                    }
                };
            }
            
            return BadRequest(transactionResponse);
        }
    }
}