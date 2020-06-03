using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// Đối tượng trả về khi gọi API sms/send
    /// </summary>
    public class SmsReport
    {
        [Required]
        [JsonProperty("cooperateMsgId")]
        public string CooperateMsgId { get; set; }

        [Required]
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        [Required]
        [JsonProperty("responeId")]
        public Guid? SmsOutGuid { get; set; }
    }
}
