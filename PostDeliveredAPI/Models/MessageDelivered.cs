using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PostDeliveredAPI.Models
{
    public class MessageDelivered
    {
        /// <summary>
        /// Id do SAMI trả
        /// </summary>
        [Required]
        [JsonProperty("smsInGuid")]
        public Guid SmsInGuid { get; set; }


        [JsonProperty("cooperateId")]
        public int? CooperateId { get; set; }

        /// <summary>
        /// Số điện thoại gửi tin nhắn đến tổng đài
        /// </summary>
        [Required]
        [JsonProperty("subscriber")]
        public string Subscriber { get; set; }

        /// <summary>
        /// Thông điệp tin nhắn
        /// </summary>
        [Required]
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Đầu số nhắn tin
        /// </summary>
        [Required]
        [JsonProperty("shortCode")]
        public string ShortCode { get; set; }


        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Thời điểm nhận tin nhắn
        /// </summary>
        [Required]
        [JsonProperty("receivedTime")]
        public DateTime ReceivedTime { get; set; }

        /// <summary>
        /// Nhà mạng
        /// </summary>
        [Required]
        [JsonProperty("operatorId")]
        public int OperatorId { get; set; }
    }
}
