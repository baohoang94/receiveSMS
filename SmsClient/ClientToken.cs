using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmsClient
{
    public class JwtPayload
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("exp")]
        [JsonRequired]
        public long Exp { get; set; }

        [JsonProperty("iss")]
        public string Issuer { get; set; }

        [JsonProperty("aud")]
        public string Audience { get; set; }
    }

    public interface ITokenClient
    {
        /// <summary>
        /// Hàm lấy token
        /// </summary>
        /// <returns></returns>
        Task<string> GetTokenAsync();

        string Url { get; }
    }

    public class TokenClient : ITokenClient
    {
        private string _token;
        private readonly string _username;
        private readonly string _password;
        private readonly string _clientId;

        public string Url { get; set; }

        public TokenClient(string url, string username, string password, string clientId)
        {
            Url = url;
            _username = username;
            _password = password;
            _clientId = clientId;
        }

        public async Task<string> GetTokenAsync()
        {
            if (IsExpired)
            {
                _token = await GetTokenAsync(_username, _password);
            }

            return _token;
        }

        private JwtPayload JwtPayload
        {
            get
            {
                if (string.IsNullOrEmpty(_token))
                {
                    return null;
                }
                else
                {
                    var tokenArray = _token.Split('.', StringSplitOptions.RemoveEmptyEntries);
                    if (tokenArray.Length != 3)
                    {
                        return null;
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<JwtPayload>(tokenArray[1]);
                    }
                }
            }
        }

        private bool IsExpired
        {
            get
            {
                if (JwtPayload == null)
                    return true;
                // Sai số 1 phút
                return JwtPayload.Exp > DateTime.UtcNow.AddMinutes(1).Ticks;
            }
        }

        private async Task<string> GetTokenAsync(string username, string password)
        {
            string resToken = null;

            try
            {
                using HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                using var client = new HttpClient(handler){};

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string authorizationBasic = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));

                // Lấy JsonWebToken
                // Basic Authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationBasic);

                // x-client-id header
                client.DefaultRequestHeaders.Add("x-client-id", _clientId);

                var result = client.PostAsync(Url, null).Result;

                Console.WriteLine("====================================");
                Console.WriteLine($"TOKEN Http POST... {Url}");
                Console.WriteLine($"Authorization Basic {authorizationBasic}");
                var resBytes = await result.Content.ReadAsByteArrayAsync();

                var resString = Encoding.UTF8.GetString(resBytes);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Loại bỏ dấu " vì kết quả trả về là một chuỗi json string dạng "token..."
                    resToken = resString.Replace("\"", "");
                    Console.WriteLine("====================================");
                    Console.WriteLine($"TOKEN Http Response code <{result.StatusCode}>");
                    Console.WriteLine($"TOKEN {resToken}");
                    Console.WriteLine("====================================");
                }
                else
                {
                    Console.WriteLine("====================================");
                    Console.WriteLine($"TOKEN Http Response code <{result.StatusCode}>");
                    Console.WriteLine($"Content {resString}");
                    Console.WriteLine("====================================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }

            return resToken;
        }
    }
}
