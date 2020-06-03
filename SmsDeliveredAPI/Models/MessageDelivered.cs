using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmsDeliveredAPI.Models
{
    public class MessageDelivered
    {
        /// <summary>
        /// Id do SAMI trả
        /// </summary>
        [Required]
        [JsonProperty("smsInGuid")]
        [Column("SmsInGuid")]
        public Guid SmsInGuid { get; set; }


        [JsonProperty("cooperateId")]
        [Column("CooperateId")]
        public int? CooperateId { get; set; }

        /// <summary>
        /// Số điện thoại gửi tin nhắn đến tổng đài
        /// </summary>
        [Required]
        [JsonProperty("subscriber")]
        [Column("Subscriber")]
        public string Subscriber { get; set; }

        /// <summary>
        /// Thông điệp tin nhắn
        /// </summary>
        [Required]
        [JsonProperty("message")]
        [Column("Message")]
        public string Message { get; set; }

        /// <summary>
        /// Đầu số nhắn tin
        /// </summary>
        [Required]
        [JsonProperty("shortCode")]
        [Column("ShortCode")]
        public string ShortCode { get; set; }


        [JsonProperty("status")]
        [Column("Status")]
        public string Status { get; set; }

        /// <summary>
        /// Thời điểm nhận tin nhắn
        /// </summary>
        [Required]
        [JsonProperty("receivedTime")]
        [Column("ReceivedTime")]
        public DateTime ReceivedTime { get; set; }

        /// <summary>
        /// Nhà mạng
        /// </summary>
        [Required]
        [JsonProperty("operatorId")]
        [Column("OperatorId")]
        public int OperatorId { get; set; }
    }
}
