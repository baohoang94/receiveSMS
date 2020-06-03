using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace PostDeliveredAPI.Models
{

    /// <summary>
    /// Lấy dữ liệu nguyên gốc từ body
    /// </summary>
    public class DeliveredTransactionRequest
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

    public class MessagesDeliveredTransaction
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
        [JsonProperty("cooperateId")]
        public int CooperateId { get; set; }

        /// <summary>
        /// Danh sách Sms cần gửi
        /// </summary>
        [Required]
        [JsonProperty("messagesDelivered")]
        public IEnumerable<MessageDelivered> MessagesDelivered { get; set; }

        /// <summary>
        /// Thời gian tạo transaction
        /// </summary>
        [Required]
        [JsonProperty("createTime")]
        public DateTime CreateTime { get; set; }
    }
}
