using System.Collections.Generic;

namespace SmsDeliveredAPI.Services
{
    public interface IAntiDuplicateDeliveredService
    {
        public void Add(string deliveredGuid);
        public void Remove(string deliveredGuid);
        public bool IsDuplicate(string deliveredGuid);
    }

    public class AntiDuplicateDeliveredService: IAntiDuplicateDeliveredService
    {
        private readonly int _max_buffer_delivered_guid = 10000;
        private readonly HashSet<string> _messageDeliveredGuid;

        public AntiDuplicateDeliveredService()
        {
            _messageDeliveredGuid = new HashSet<string>();
        }
        public void Add(string deliveredGuid)
        {
            if (_messageDeliveredGuid.Count > _max_buffer_delivered_guid)
                _messageDeliveredGuid.Clear();
            _messageDeliveredGuid.Add(deliveredGuid);
        }
        public void Remove(string deliveredGuid)
        {
            if (_messageDeliveredGuid.Contains(deliveredGuid)) _messageDeliveredGuid.Remove(deliveredGuid);
        }
        public bool IsDuplicate(string deliveredGuid)
        {
            return _messageDeliveredGuid.Contains(deliveredGuid);
        }
    }
}
