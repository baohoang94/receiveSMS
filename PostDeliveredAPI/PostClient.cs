using Newtonsoft.Json;
using PostDeliveredAPI.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PostDeliveredAPI
{
    public interface IClienSend
    {
        Task<string> SendAsync(byte[] bodyRawBytes);
        bool IsAlive();
        string Url { get; set; }
    }

    public class PostClient : IClienSend
    {
        public string Url { get; set; }

        public PostClient(string url)
        {
            Url = url;
        }

        public bool IsAlive()
        {
            using HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using var client = new HttpClient(handler) { };

            var result = client.GetAsync(Url).Result;

            // Kết quả thành công sẽ trả về http Status Code 200
            return (result.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Hàm gửi ban tin smsIn (MO)
        /// </summary>
        /// <param name="bodyRawBytes">Chuỗi bytes của body</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public async Task<string> SendAsync(byte[] bodyRawBytes)
        {
            string resString = null;

            using HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var client = new HttpClient(handler){})
            {
              
                var byteArrayContent = new ByteArrayContent(bodyRawBytes);

                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await client.PostAsync(Url, byteArrayContent);

                Console.WriteLine("====================================");
                Console.WriteLine($"Http Response code {result.StatusCode}");

                var resBytes = await result.Content.ReadAsByteArrayAsync();

                resString = Encoding.UTF8.GetString(resBytes);
                Console.WriteLine("====================================");
                Console.WriteLine(resString);
                Console.WriteLine("====================================");

                // Kết quả thành công sẽ trả về json object
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Chuyển đổi json object thành TransactionResponse
                    var transResponse = JsonConvert.DeserializeObject<DeliveredTransactionResponse>(resString);
                }
            }

            return resString;
        }
    }
}
