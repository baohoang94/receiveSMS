using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    [Serializable]
    [JsonObject("smsOut")]
    public class SmsOut
    {
        /// <summary>
        /// ID của tin nhắn phía đối tác
        /// </summary>
        [Key]
        [JsonProperty("cooperateMsgId")]
        public string CooperateMsgId { get; set; }

        /// <summary>
        /// Số điện thoại gửi Message
        /// </summary>
        [Required]
        [JsonProperty("destAddr")]
        public string DestAddr { get; set; }

        /// <summary>
        /// Nội dung Message cần gửi
        /// </summary>
        [Required]
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// ShortCode hoặc Brandname cần gửi
        /// </summary>
        [Required]
        [JsonProperty("shortCode")]
        public string ShortCode { get; set; }

        /// <summary>
        /// Loại MT gửi đi
        /// Mặc định là 2 (Announcement)
        /// Bắt buộc phải chỉ định là loại tin nào
        /// Để đảm bảo rằng Đối tác nhận thức đúng về hình thức gửi tin
        /// </summary>
        [Required]
        [JsonProperty("mtType")]
        public string MtType { get; set; }

        /// <summary>
        /// MT này trả lời tin nhắn cho MO nào
        /// Khi không có MO thì giá trị này là null
        /// </summary>
        [JsonProperty("smsInGuid")]
        public Guid? SmsInGuid { get; set; }

        /// <summary>
        /// Nội dung trường chỉ ra MT này có tính cước cho MO không.
        /// Giá trị là DATA (Tính cước) hoặc FREE (không tính cước) hoặc WRONG (Tin sai)
        /// </summary>
        [Required]
        [JsonProperty("cdrIndicator")]
        public string CdrIndicator { get; set; }


        /// <summary>
        /// Trường này dùng để thống kê.
        /// MT này trả lơi cho MO nào theo phân loại lệnh
        /// </summary>
        /// 
        [JsonProperty("commandId")]
        public int? CommandId { get; set; }

        [JsonProperty("operatorId")]
        public int? OperatorId { get; set; }
    }

    
}
