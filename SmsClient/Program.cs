using System;
using API.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace SmsClient
{
    class Program
    {
        /// <summary>
        /// Demo send MT message
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            // clientId - clientId của hệ thống API gửi sms, luôn luôn là "efa66179-1eb9-4187-9c0f-52fc99388492"
            const string clientId = "efa66179-1eb9-4187-9c0f-52fc99388492";
            // User Name
            const string userName = "account@your.company.com";
            // Password
            const string password = "yourpassword";
            // Đường dẫn tới PrivateKey của bạn (Dùng để ký dữ liệu gửi đi)
            const string privateKeyPath = @".\PrivateKey\your.company.key";
            // Mã định danh của đối tác (Thay 0 bằng mã định danh của công ty bạn được SAMI cấp)
            const int cooperateId = 0;

            // Lớp lấy token
            ITokenClient clientToken = new TokenClient("https://auth.sami.vn:8443/api/authenticate/token", userName, password, clientId); 

            // Lớp gửi SMS
            ISmsClient smsClient = new SmsClient(@"https://sms.sami.vn:8558/api/sms/send");

            // Nạp khóa Private của đối tác
            var provider = Crypto.PemKeyUtils.GetRSAProviderFromPemFile(privateKeyPath);

            // Chuẩn bị dữ liệu đầu vào
            List<SmsOut> smsOuts = new List<SmsOut>();

            //// Gui tin qua ShortCode phan hoi cho mot ban tin MO - trong vi du nay la 8150
            //// Bat buoc phai co SmsInGuid, vi tren dau so ngan khong duoc gui MT chu dong
            //// SmsInGuid la truong SAMI chuyen qua ban tin SmsIn (MO)
            //SmsOut smsShortCodeReplyMo = new SmsOut
            //{
            //    CooperateMsgId = Guid.NewGuid().ToString(),
            //    DestAddr = "84912656901",
            //    Message = "Test Message",
            //    ShortCode = "8150", // Day la dau so ngan
            //    CdrIndicator = "FREE",
            //    MtType = "AN",
            //    // Doi voi truong hop SmsOut tra loi cho SmsIn (MO) thi bat buoc phai co SmsInGuid
            //    // SmsInGuid do SAMI tra tu ban tin SmsIn
            //    SmsInGuid = Guid.Parse("90acb77f-0a02-4e10-b543-cf108ed20223"),
            //    // OperatorId cung do SAMI tra tu ban tin SmsIn
            //    OperatorId = 2
            //};

            //smsOuts.Add(smsShortCodeReplyMo);

            //// Gui brandname phan hoi cho mot ban tin MO - trong vi du nay la Panasonic
            //SmsOut smsBrandnameReplyMo = new SmsOut
            //{
            //    CooperateMsgId = Guid.NewGuid().ToString(),
            //    DestAddr = "84912656901",
            //    Message = "Luu y ban tin test phai dung mau da dang ky",
            //    ShortCode = "Panasonic", // Day la dau so ngan
            //    CdrIndicator = "FREE",
            //    MtType = "AN",
            //    // SmsInGuid do SAMI tra tu ban tin SmsIn
            //    SmsInGuid = Guid.Parse("90acb77f-0a02-4e10-b543-cf108ed20223"),
            //    // OperatorId cung do SAMI tra tu ban tin SmsIn
            //    OperatorId = 2
            //};
            //smsOuts.Add(smsBrandnameReplyMo);

            // Truong hop gui tin nhan Brandname khong phan hoi cho MO nao - trong vi du nay la Panasonic
            SmsOut smsBrandname = new SmsOut
            {
                CooperateMsgId = Guid.NewGuid().ToString(),
                DestAddr = "84912656901",
                Message = "Luu y ban tin test phai dung mau da dang ky",
                ShortCode = "Panasonic",
                CdrIndicator = "FREE",
                MtType = "AN"
            };
            smsOuts.Add(smsBrandname);

            // Tạo transaction
            SmsTransaction smsTransaction = new SmsTransaction
            {
                // ID giao dịch
                TransactionId = Guid.NewGuid().ToString(),
                // Mã định danh của đối tác
                CoopereateId = cooperateId,
                // Thời gian tạo transaction
                CreateTime = DateTime.Now,
                // Danh sách Sms cần gửi
                SmsOuts = smsOuts
            };

            // Chuyển transaction sang chuỗi Json
            string jsonTran = JsonConvert.SerializeObject(smsTransaction);

            // Chuyển chuỗi json sang bytes để tiến hành ký
            // Chú ý sử dụng Encoding UTF8
            var byteData = Encoding.UTF8.GetBytes(jsonTran);

            // Chuyển byteData sang Base64 để đưa vào payload
            var payload = Convert.ToBase64String(byteData);

            Crypto.IRsaCrypto rsaCrypto = new Crypto.RsaCrypto();
            // Ký vào byteData sử dụng khóa Private do provider cung cấp
            var signature = Convert.ToBase64String(rsaCrypto.SignHash(byteData, provider));

            // Chuẩn bị transaction
            SmsTransactionRequest smsTransactionRequest = new SmsTransactionRequest
            {
                Payload = payload,
                Signature = signature
            };

            // Chuyển transaction thành JSON
            string jsonTransactionRequest = JsonConvert.SerializeObject(smsTransactionRequest);

            // Chuyển jsonTransactionRequest thành dạng bytes mã hóa UTF8
            byte[] bodyRawBytes = Encoding.UTF8.GetBytes(jsonTransactionRequest);

            // Kiểm tra token còn hạn không, nếu không còn hạn sử dụng thì lấy token mới
            // Hiện chúng tôi đang để token sẽ hết hạn trong vòng 5 phut
            var token = await clientToken.GetTokenAsync();

            if (token != null)
            {
                // Bạn nên log mỗi cuộc gọi và kết quả trả về
                var response = await smsClient.SendAsync(bodyRawBytes, token);

                Console.WriteLine("====================================");
                Console.WriteLine($"Http Response code {response.StatusCode}");

                var resBytes = await response.Content.ReadAsByteArrayAsync();

                var resString = Encoding.UTF8.GetString(resBytes);
                Console.WriteLine("====================================");
                Console.WriteLine(resString);
                Console.WriteLine("====================================");

                // Kết quả thành công sẽ trả về json object
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Chuyển đổi json object thành TransactionResponse
                    var transResponse = JsonConvert.DeserializeObject<SmsTransactionResponse>(resString);
                    // Sau khi có response, bạn sẽ xử lý các nghiệp vụ tiếp theo ở dưới
                    // Cập nhật vào database v...v
                } else
                {
                    // Ghi log loi
                }
            }
            else
            {
                Console.WriteLine("ERROR: Token is Empty");
            }

            Console.ReadKey();
        }
    }
}
