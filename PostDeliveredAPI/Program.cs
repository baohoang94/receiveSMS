using Newtonsoft.Json;
using PostDeliveredAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PostDeliveredAPI
{
    class Program
    {
        /// <summary>
        /// Test Delivered API
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            Console.WriteLine("Test Delivered API");

            var postClient = new PostClient("https://localhost:8444/api/delivered/messages");

            // Nạp khóa Private
            var provider = Crypto.PemKeyUtils.GetRSAProviderFromPemFile(@".\PrivateKey\test-message-delivered.key");

            // Chuẩn bị dữ liệu đầu vào
            List<MessageDelivered> messagesDelivered = new List<MessageDelivered>();

            // Tao ban tin SMS
            MessageDelivered delivered = new MessageDelivered
            {
                SmsInGuid = Guid.NewGuid(),
                Message = "tesst",
                ShortCode = "8050",
                Subscriber = "94912656901",
                OperatorId = 1,
                ReceivedTime  = DateTime.Now,
                CooperateId = 46045,
                Status = "PENDING"
            };

            messagesDelivered.Add(delivered);

            // Tạo transaction
            MessagesDeliveredTransaction messagesDeliveredTransaction = new MessagesDeliveredTransaction
            {
                // ID giao dịch
                TransactionId = Guid.NewGuid().ToString(),
                // Mã định danh của đối tác
                CooperateId = 46044,
                // Thời gian tạo transaction
                CreateTime = DateTime.Now,
                // Danh sách Delivered cần gửi
                MessagesDelivered = messagesDelivered
            };

            // Chuyển transaction sang chuỗi Json
            string jsonTran = JsonConvert.SerializeObject(messagesDeliveredTransaction);

            // Chuyển chuỗi json sang bytes để tiến hành ký
            // Chú ý sử dụng Encoding UTF8
            var byteData = Encoding.UTF8.GetBytes(jsonTran);

            // Chuyển byteData sang Base64 để đưa vào payload
            var payload = Convert.ToBase64String(byteData);

            Crypto.IRsaCrypto rsaCrypto = new Crypto.RsaCrypto();
            // Ký vào byteData sử dụng khóa Private do provider cung cấp
            var signature = Convert.ToBase64String(rsaCrypto.SignHash(byteData, provider));

            // Chuẩn bị transaction
            DeliveredTransactionRequest deliveredTransactionRequest = new DeliveredTransactionRequest
            {
                Payload = payload,
                Signature = signature
            };

            // Chuyển transaction thành JSON
            string jsonTransactionRequest = JsonConvert.SerializeObject(deliveredTransactionRequest);

            // Chuyển jsonTransactionRequest thành dạng bytes mã hóa UTF8
            byte[] bodyRawBytes = Encoding.UTF8.GetBytes(jsonTransactionRequest);

           if(postClient.IsAlive())
            {
                // Bạn nên log mỗi cuộc gọi và kết quả trả về
                var response = await postClient.SendAsync(bodyRawBytes);
            } else
            {
                Console.WriteLine($"Url {postClient.Url} is not available");
            }
        }
    }
}
