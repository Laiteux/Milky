using Milky.Enums;
using System;
using System.Net;

namespace Milky.Models
{
    public class ProxySettings
    {
        public ProxySettings(ProxyProtocol protocol)
        {
            Protocol = protocol;
        }

        public ProxyProtocol Protocol { get; } = ProxyProtocol.HTTP;

        /// <summary>
        /// This is required for backconnect/rotating proxies since IPs won't be "rotating" otherwise, because of the socket connection being kept open
        /// </summary>
        public bool Backconnect { get; set; } = false;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Whether or not to follow redirections
        /// </summary>
        public bool AllowAutoRedirect { get; set; } = true;

        /// <summary>
        /// Whether or not to use/save/keep cookies, usually not recommended for credential stuffing but who knows
        /// </summary>
        public bool UseCookies { get; set; } = false;

        /// <summary>
        /// Default <see cref="System.Net.CookieContainer"/>, I see no use case for it for credential stuffing but who knows
        /// </summary>
        public CookieContainer CookieContainer { get; set; } = null;
    }
}
