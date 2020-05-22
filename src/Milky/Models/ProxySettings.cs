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

        public ProxyProtocol Protocol { get; private set; } = ProxyProtocol.HTTP;

        public bool Backconnect { get; set; } = false;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
