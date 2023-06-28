using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;


namespace FoodDefence.Models.Repository
{
    public class Security
    {
        static readonly string pass = "Zfme$%23anej$ERAP5232ds-%43$rwFzNJHYUhghjdfIYjh";

        public string Encrypt(string txt)
        {
            if (txt == null) { return ""; }
            var bytesEncryt = Encoding.UTF8.GetBytes(txt);
            var passBytes = Encoding.UTF8.GetBytes(pass);

            passBytes = SHA512.Create().ComputeHash(passBytes);

            return Convert.ToBase64String(Encrypt(bytesEncryt, passBytes));
        }

        public string Decrypt(string txt)
        {
            if (txt == null) { return ""; }
            var bytesEncryt = Convert.FromBase64String(txt);
            var passBytes = Encoding.UTF8.GetBytes(pass);

            passBytes = SHA512.Create().ComputeHash(passBytes);

            return Encoding.UTF8.GetString(Decrypt(bytesEncryt, passBytes));
        }

        private static byte[] Encrypt(byte[] byToEncryt, byte[] passBytes)
        {
            byte[] resultBytes = null;
            var saltByte = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    var k = new Rfc2898DeriveBytes(passBytes, saltByte, 1000);
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = k.GetBytes(aes.KeySize / 8);
                    aes.IV = k.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(byToEncryt, 0, byToEncryt.Length);
                        cs.Close();
                    }

                    resultBytes = ms.ToArray();
                }
            }
            return resultBytes;

        }

        private static byte[] Decrypt(byte[] byToEncryt, byte[] passBytes)
        {
            byte[] resultBytes = null;
            var saltByte = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    var k = new Rfc2898DeriveBytes(passBytes, saltByte, 1000);
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = k.GetBytes(aes.KeySize / 8);
                    aes.IV = k.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(byToEncryt, 0, byToEncryt.Length);
                        cs.Close();
                    }

                    resultBytes = ms.ToArray();
                }
            }
            return resultBytes;

        }
    }
}