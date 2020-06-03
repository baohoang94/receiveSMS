using System;
using System.Security.Cryptography;
using System.Text;

namespace Crypto
{
    public enum HashAlgorithm
    {
        MD5, RIPEMD160, SHA1, SHA256, SHA384, SHA512
    }
    public interface IRsaCrypto
    {
        /// <summary>
        /// Mã hóa plainText sử dụng PublicKey
        /// </summary>
        /// <param name="plainText">Nội dung mã hóa</param>
        /// <param name="cryptoServiceProvider">Chứa PublicKey</param>
        /// <returns></returns>
        byte[] Encrypt(string plainText, RSACryptoServiceProvider cryptoServiceProvider);

        /// <summary>
        /// Giải mã một chuỗi đã mã hóa bằng PrivateKey
        /// </summary>
        /// <param name="cipherText">Chuỗi đã mã hóa bằng PublicKey</param>
        /// <param name="cryptoServiceProvider">Chứa PrivateKey</param>
        /// <returns>Chuỗi giải mã</returns>
        string Decrypt(string cipherText, RSACryptoServiceProvider cryptoServiceProvider);

        /// <summary>
        /// Xác minh chữ ký dựa trên PublicKey
        /// </summary>
        /// <param name="signature">Chuỗi bytes chữ ký</param>
        /// <param name="byteData">Dữ liệu cần xác minh cùng với chữ ký signature. 
        /// data sẽ được chuyển sang dưới dạng UTF8 bytes
        /// </param>
        /// <param name="cryptoServiceProvider">Chứa PublicKey</param>
        /// <returns></returns>
        bool VerifyHashData(byte[] signature, byte[] byteData, RSACryptoServiceProvider cryptoServiceProvider);

        /// <summary>
        /// Sử dụng PrivateKey để ký vào dữ liệu
        /// Thuật toán Hash sử dụng là SHA256
        /// </summary>
        /// <param name="dataToSign">Bytes dữ liệu cần ký</param>
        /// <param name="cryptoServiceProvider">Chứa PrivateKey</param>
        /// <returns>Chuỗi bytes chữ ký</returns>
        byte[] SignHash(byte[] dataToSign, RSACryptoServiceProvider cryptoServiceProvider);

    }

    public class RsaCrypto: IRsaCrypto
    {
        public RsaCrypto()
        {
        }

        public byte[] Encrypt(string plainText, RSACryptoServiceProvider cryptoServiceProvider)
        {
            byte[] encrypted;

            //for encryption, always handle bytes...
            var bytesPlainTextData = Encoding.UTF8.GetBytes(plainText);

            //apply pkcs#1.5 padding and encrypt our data 
            encrypted = cryptoServiceProvider.Encrypt(bytesPlainTextData, false);

            return encrypted;
        }

        public string Decrypt(string cipherText, RSACryptoServiceProvider cryptoServiceProvider)
        {

            //first, get our bytes back from the base64 string ...
            var bytesCypherText = Convert.FromBase64String(cipherText);

            //decrypt and strip pkcs#1.5 padding
            var bytesPlainTextData = cryptoServiceProvider.Decrypt(bytesCypherText, false);

            //get our original plainText back...
            string plaintext = Encoding.UTF8.GetString(bytesPlainTextData);

            return plaintext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signature">Chữ ký</param>
        /// <param name="byteData"></param>
        /// <param name="cryptoServiceProvider"></param>
        /// <returns></returns>
        public bool VerifyHashData(byte[] signature, byte[] byteData, RSACryptoServiceProvider cryptoServiceProvider)
        {
            // Mặc định ký vào chuỗi dữ liệu format là UTF8 nên phải GetDataHash bằng UTF8
            var messageHash = GetDataHash(byteData, HashAlgorithm.SHA256);

            return cryptoServiceProvider.VerifyHash(messageHash, CryptoConfig.MapNameToOID("SHA256"), signature);
        }

        /// <summary>
        /// Ký một chuỗi ký tự
        /// </summary>
        /// <param name="dataToSign">Bytes dữ liệu cần ký</param>
        /// <param name="cryptoServiceProvider">Chứa PrivateKey để ký</param>
        /// <returns></returns>
        public byte[] SignHash(byte[] dataToSign, RSACryptoServiceProvider cryptoServiceProvider)
        {
            // Hash bằng thuật toán SHA256
            byte[] hashValue;
            using (SHA256 mySHA256 = SHA256.Create())
            {
                hashValue = mySHA256.ComputeHash(dataToSign);
            }

            //// Ký vào Hash
            //byte[] signature = rsa.SignHash(hashValue, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            byte[] signature = cryptoServiceProvider.SignHash(hashValue, CryptoConfig.MapNameToOID("SHA256"));

            return signature;
        }
        private static byte[] GetDataHash(byte[] byteData, HashAlgorithm hashAlgorithm = HashAlgorithm.SHA256)
        {
            //choose any hash algorithm
            //SHA1Managed managedHash = new SHA1Managed();
            //return managedHash.ComputeHash(Encoding.Unicode.GetBytes(sampleData));
            // Create a SHA256
            switch (hashAlgorithm)
            {
                case HashAlgorithm.MD5:
                    using (MD5 md5Hash = MD5.Create())
                    {
                        // ComputeHash - returns byte array  
                        return md5Hash.ComputeHash(byteData);
                    }
                case HashAlgorithm.RIPEMD160:
                    using (RIPEMD160 ripemd160Hash = new RIPEMD160Managed())
                    {
                        // ComputeHash - returns byte array  
                        return ripemd160Hash.ComputeHash(byteData);
                    }
                case HashAlgorithm.SHA1:
                    using (SHA1 sha1Hash = SHA1.Create())
                    {
                        // ComputeHash - returns byte array  
                        return sha1Hash.ComputeHash(byteData);
                    }
                case HashAlgorithm.SHA256:
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        // ComputeHash - returns byte array  
                        return sha256Hash.ComputeHash(byteData);
                    }
                case HashAlgorithm.SHA384:
                    using (SHA384 sha384Hash = SHA384.Create())
                    {
                        // ComputeHash - returns byte array  
                        return sha384Hash.ComputeHash(byteData);
                    }
                case HashAlgorithm.SHA512:
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        // ComputeHash - returns byte array  
                        return sha256Hash.ComputeHash(byteData);
                    }
                default:
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        // ComputeHash - returns byte array  
                        return sha256Hash.ComputeHash(byteData);
                    }
            }
        }
    }
}
