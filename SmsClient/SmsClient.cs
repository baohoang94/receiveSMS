using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SmsClient
{
    public interface ISmsClient
    {
        Task<HttpResponseMessage> SendAsync(byte[] bodyRawBytes, string token);

        string SmsApiPath { get; }
    }

    public class SmsClient : ISmsClient
    {
        public string Url { get; set; }

        public SmsClient(string url)
        {
            Url = url;
        }

        public string SmsApiPath { get; }

        /// <summary>
        /// Hàm gửi SMS
        /// </summary>
        /// <param name="bodyRawBytes">Chuỗi bytes của body</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendAsync(byte[] bodyRawBytes, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Token must not null or empty!");

            using HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };


            using var client = new HttpClient(handler){};

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Console.WriteLine("====================================");
            Console.WriteLine($"Http POST {SmsApiPath}");
            Console.WriteLine($"Authorization Bearer {token}");

            // Add Header Bearer token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var byteArrayContent = new ByteArrayContent(bodyRawBytes);

            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await client.PostAsync(SmsApiPath, byteArrayContent);
        }
    }
}
