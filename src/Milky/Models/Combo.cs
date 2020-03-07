using System;

namespace Milky.Models
{
    public class Combo
    {
        public Combo(string combo)
        {
            string[] split = combo.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

            Username = split[0];
            Password = split[1];
        }

        public Combo(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public override string ToString()
        {
            return string.Join(":", Username, Password);
        }
    }
}
