using SmsDeliveredAPI.Models;
using System;
using System.Collections.Generic;

namespace SmsDeliveredAPI.Services
{
    public interface IMessagesDeliveredService
    {
        /// <summary>
        /// Kết quả xử lý Insert các bản tin MT vào database
        /// </summary>
        List<DeliveredReport> DeliveredReports { get;}

        void PushMessages(IEnumerable<MessageDelivered> messagesDelivered);

        /// <summary>
        /// Lưu các bản tin MT vào Database để gửi đi
        /// </summary>
        /// <returns></returns>
        bool Save();
    }
    public class MessagesDeliveredService: IMessagesDeliveredService
    {
        private readonly IAntiDuplicateDeliveredService _antiDuplicateDeliveredService;

        private readonly IMessageDeliveredRepository _messagesDelivered;

        public List<DeliveredReport> DeliveredReports { get; private set; }


        public MessagesDeliveredService(IMessageDeliveredRepository messagesDelivered,
            IAntiDuplicateDeliveredService antiDuplicateDeliveredService)
        {
            DeliveredReports = new List<DeliveredReport>();
            _messagesDelivered = messagesDelivered;
            _antiDuplicateDeliveredService = antiDuplicateDeliveredService;
        }

        /// <summary>
        /// Insert Message
        /// </summary>
        /// <param name="messagesDelivered"></param>
        public void PushMessages(IEnumerable<MessageDelivered> messagesDelivered)
        {
            foreach (var delivered in messagesDelivered)
            {
                DeliveredReport deliveredReport;
                if (_antiDuplicateDeliveredService.IsDuplicate(delivered.SmsInGuid.ToString()))
                {
                    // Thông báo sẽ Response cho đối tác SAMI
                    deliveredReport = new DeliveredReport
                    {
                        DeliveredGuid = null,
                        SmsInGuid = delivered.SmsInGuid.ToString(),
                        StatusCode = 2,
                        StatusMessage = "DUPLICATE"
                    };

                    DeliveredReports.Add(deliveredReport);
                    continue;
                }

                _antiDuplicateDeliveredService.Add(delivered.SmsInGuid.ToString());

                // CHUẨN BỊ DỮ LIỆU ĐỂ INSERT vào DATABASE và trả thông báo về cho đối tác
                // Tạo khóa đại diện duy nhất
                // Để tiện Insert vào DB và trả SmsOutGuid về cho khách hàng,
                // ở đây sẽ sử dụng Guid để sinh ra khóa duy nhất
                var deliveredGuid = Guid.NewGuid();

                // Thông báo sẽ Response cho đối tác
                deliveredReport = new DeliveredReport
                {
                    DeliveredGuid = deliveredGuid.ToString(),
                    SmsInGuid = delivered.SmsInGuid.ToString(),
                    StatusCode = 0,
                    StatusMessage = "RECEIVED"
                };

                DeliveredReports.Add(deliveredReport);

                // Đối tượng lưu vào database
                MessageDeliveredWaiting messageDeliveredWaiting = new MessageDeliveredWaiting
                {
                    DeliveredGuid = deliveredGuid,
                    SmsInGuid = delivered.SmsInGuid,
                    Message = delivered.Message,
                    ReceivedTime = delivered.ReceivedTime,
                    OperatorId = delivered.OperatorId,
                    ShortCode = delivered.ShortCode,
                    Subscriber = delivered.Subscriber,
                    CooperateId = delivered.CooperateId,
                    Status = delivered.Status
                };

                _messagesDelivered.AddMessageDelivered(messageDeliveredWaiting);

            }
        }

        /// <summary>
        /// Save Message to Database
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            return _messagesDelivered.SaveChanges();
        }
    }
}
