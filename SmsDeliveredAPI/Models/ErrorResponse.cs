using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmsDeliveredAPI.Models
{
    /// <summary>
    /// Đối tượng lỗi trả về
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// Thông điệp lỗi
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
