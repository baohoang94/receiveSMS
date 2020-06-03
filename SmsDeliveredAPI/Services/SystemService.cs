using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Schema;
using System.IO;
using System.Security.Cryptography;

namespace SmsDeliveredAPI.Services
{
    public interface ISystemService
    {
        RSACryptoServiceProvider SamiPublicKey { get;  }
        JSchema DeliveredTransactionSchema { get; }
    }


    public sealed class SystemService: ISystemService
    {
        /// <summary>
        /// Danh sách khóa Public của đối tác gửi bản tin MO - trong trường hợp này là SAMI-S
        /// </summary>
        public RSACryptoServiceProvider SamiPublicKey{ get; private set; }

        public SystemService(IConfiguration config)
        {
            DeliveredTransactionSchema = JSchema.Parse(File.ReadAllText(config.GetSection("DeliveredJsonSchema")
                .GetSection("DeliveredTransactionSchema").Value));
            SamiPublicKey = ReadSamiPublicKey(config.GetSection("PublicKey")
                .GetSection("SAMI").Value);
        }

        /// <summary>
        /// Sử dụng để validate transaction
        /// </summary>
        public JSchema DeliveredTransactionSchema { get; }

        /// <summary>
        /// Load Cooperate Public Key from file
        /// </summary>
        /// <param name="cooperateId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private RSACryptoServiceProvider ReadSamiPublicKey(string fileName)
        {
            RSACryptoServiceProvider cryptoServiceProvider = Crypto.PemKeyUtils.GetRSAProviderFromPemFile(fileName);
            return cryptoServiceProvider;
        }

    }
}
