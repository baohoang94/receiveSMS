using System;
using System.Collections.Generic;

namespace PostDeliveredAPI.Models
{
    internal class DeliveredTransactionResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public DeliveredData Data { get; set; }
        public DateTime ResponseTime { get; set; }
    }

    internal class DeliveredData
    {
        public string TransactionID { get; set; }
        public List<DeliveredReport> DeliveredReports { get; set; }
    }
}