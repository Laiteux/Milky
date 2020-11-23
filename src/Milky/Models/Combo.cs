using System;

namespace Milky.Models
{
    public class Combo
    {
        private const int SplitCount = 2;

        public Combo(string combo, string separator = ":")
        {
            Separator = separator;

            string[] split = combo.Split(separator, SplitCount, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length != SplitCount)
            {
                return;
            }

            Username = split[0];
            Password = split[1];

            Valid = true;
        }

        internal bool Valid { get; }

        public string Username { get; }

        public string Password { get; }

        private string Separator { get; }

        public override string ToString()
        {
            return string.Join(Separator, Username, Password);
        }
    }
}
