using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace API.Models
{
    /// <summary>
    /// Lấy dữ liệu nguyên gốc từ body
    /// </summary>
    public class SmsTransactionRequest
    {
        /// <summary>
        /// Payload
        /// </summary>
        [Required]
        [JsonProperty("payload")]
        [JsonRequired]
        public string Payload { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        [Required]
        [JsonProperty("signature")]
        [JsonRequired]
        public string Signature { get; set; }
    }


    /// <summary>
    /// Thực hiện giải mã SmsTransactionRequest để đưa vào SmsTransaction
    /// </summary>
    public class SmsTransaction
    {
        /// <summary>
        /// ID Giao dịch
        /// </summary>
        [Required]
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Mã định danh của đối tác
        /// </summary>
        [Required]
        [JsonProperty("coopereateId")]
        public int CoopereateId { get; set; }

        /// <summary>
        /// Danh sách Sms cần gửi
        /// </summary>
        [Required]
        [JsonProperty("smsOuts")]
        public IEnumerable<SmsOut> SmsOuts { get; set; }

        /// <summary>
        /// Thời gian tạo transaction
        /// </summary>
        [Required]
        [JsonProperty("createTime")]
        public DateTime CreateTime { get; set; }
    }
}
