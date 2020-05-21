using System;

namespace Milky.Models
{
    public class Combo
    {
        private static readonly int _splitCount = 2;

        public Combo(string combo, char separator = ':')
        {
            string[] split = combo.Split(separator, _splitCount, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length != _splitCount)
            {
                return;
            }

            Username = split[0];
            Password = split[1];

            IsValid = true;
        }

        internal bool IsValid { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public override string ToString()
        {
            return string.Join(":", Username, Password);
        }
    }
}
