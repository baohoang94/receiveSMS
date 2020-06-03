using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostDeliveredAPI.Models
{
    public class DeliveredReport
    {
        [Required]
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        /// <summary>
        /// ID ghi nhan tu phia SAMI chuyen sang
        /// </summary>
        [Required]
        [JsonProperty("smsInGuid")]
        public string SmsInGuid { get; set; }

        /// <summary>
        /// ID ghi nhan tu phia doi tac
        /// </summary>
        [Required]
        [JsonProperty("responeId")]
        public string DeliveredGuid { get; set; }
    }
}
