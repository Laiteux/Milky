using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Milky.Utils
{
    public class StringUtils
    {
        public readonly string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXY";
        public readonly string lowercaseLetters = "abcdefghijklmnopqrstuvwxy";
        public readonly string numbers = "0123456789";

        private readonly Random random = new Random();

        public string RandomString(int length, string characters)
        {
            string text = null;

            for (int i = 0; i < length; i++)
                text += characters[random.Next(characters.Length)];

            return text;
        }

        public string RandomIPV4address()
        {
            return $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}";
        }

        public string Escape(string text)
        {
            return Regex.Escape(text);
        }

        public string Unescape(string text)
        {
            return Regex.Unescape(text);
        }

        public string EncodeBase64(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        public string DecodeBase64(string text)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }

        public int CountOccurrences(string text, string find)
        {
            return text.Split(new string[] { find }, StringSplitOptions.None).Length - 1;
        }

        private static StringUtils _classInstance;
        public static StringUtils GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new StringUtils());
        }
    }
}