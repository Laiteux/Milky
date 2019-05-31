using System.Security.Cryptography;
using System.Text;

namespace Milky.Utils
{
    public class HashUtils
    {
        public string CreateMD5(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(text));
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString("x2"));

                return sb.ToString();
            }
        }

        private static HashUtils _classInstance;
        public static HashUtils GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new HashUtils());
        }
    }
}