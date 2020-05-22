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

        public ProxyProtocol Protocol { get; private set; } = ProxyProtocol.HTTP;

        public bool Backconnect { get; set; } = false;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        public bool AllowAutoRedirect { get; set; } = true;

        public bool UseCookies { get; set; } = false;

        public CookieContainer CookieContainer { get; set; } = null;
    }
}
