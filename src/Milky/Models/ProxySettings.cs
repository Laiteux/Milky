using Milky.Enums;
using System;

namespace Milky.Models
{
    public class ProxySettings
    {
        public ProxySettings(ProxyProtocol protocol)
        {
            Protocol = protocol;
        }

        public ProxyProtocol Protocol { get; }

        /// <summary>
        /// This is required for rotating proxies, otherwise the IP won't rotate because of the socket connection being kept open
        /// </summary>
        public bool Rotating { get; set; } = false;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}