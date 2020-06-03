using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace API.Models
{
    public class SmsTransactionResponse
    {
        [JsonProperty("transactionId")]
        public string TransactionID { get; set; }

        [JsonProperty("responseTime")]
        public DateTime ResponseTime { get; set; }

        [JsonProperty("smsReports")]
        public IEnumerable<SmsReport> SmsReports { get; set; }
    }
}
