using Milky.Enums;
using System;

namespace Milky.Models
{
    public class ProxySettings
    {
        public ProxyProtocol Protocol { get; set; } = ProxyProtocol.HTTP;

        public bool Backconnect { get; set; } = false;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
