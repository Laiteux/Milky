using System;

namespace Milky.Models
{
    public class Combo
    {
        private static readonly int _splitCount = 2;

        public Combo(string combo, string separator = ":")
        {
            string[] split = combo.Split(separator, _splitCount, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length != _splitCount)
            {
                return;
            }

            Username = split[0];
            Password = split[1];
            Separator = separator;
            IsValid = true;
        }

        internal bool IsValid { get; }

        public string Username { get; }

        public string Password { get; }

        private string Separator { get; }

        public override string ToString()
        {
            return string.Join(Separator, Username, Password);
        }
    }
}
