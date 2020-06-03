using Microsoft.EntityFrameworkCore;
using SmsDeliveredAPI.Extensions;
using System;
using System.Collections.Generic;

namespace SmsDeliveredAPI.Models
{
    public interface IMessageDeliveredRepository
    {
        void AddMessageDelivered(MessageDeliveredWaiting messageDelivered);

        void AddRangeMessageDelivered(IEnumerable<MessageDeliveredWaiting> messagesDelivered);

        bool SaveChanges();
    }

    public class MessageDeliveredWaitingRepository : IMessageDeliveredRepository
    {
        private readonly DbContext _dbContext;

        private readonly List<MessageDeliveredWaiting> _messageDeliveredWaitings;

        public MessageDeliveredWaitingRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _messageDeliveredWaitings = new List<MessageDeliveredWaiting>();
        }

        public void AddMessageDelivered(MessageDeliveredWaiting messageDelivered)
        {
            _messageDeliveredWaitings.Add(messageDelivered);
        }

        public void AddRangeMessageDelivered(IEnumerable<MessageDeliveredWaiting> messagesDelivered)
        {
            _messageDeliveredWaitings.AddRange(messagesDelivered);
        }

        public bool SaveChanges()
        {
            if(_messageDeliveredWaitings.Count > 0)
            {
                bool executeResult = Helpers.Retry.Execute(() => _dbContext.BulkInsert(_messageDeliveredWaitings, "MessageDeliveredWaiting"), retryInterval: new TimeSpan(hours: 0, minutes: 0, seconds: 1), retryCount: 3, expectedResult: true, isExpectedResultEqual: true, isSuppressException: false);

                return executeResult;
            }
            return false;
        }
    }
}
