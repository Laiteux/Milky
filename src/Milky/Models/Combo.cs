namespace Milky.Models
{
    public class Combo
    {
        /// <summary>
        /// The combo username part (before the colon)
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The combo password part (after the colon)
        /// </summary>
        public string Password { get; set; }

        /// <returns><see cref="Username"/> and <see cref="Password"/> separated by a colon (Username:Password)</returns>
        public override string ToString()
        {
            return string.Join(":", Username, Password);
        }
    }
}
