using System.Net;
using System.Net.Http;

namespace Milky.Models
{
    public class Proxy
    {
        public Proxy(string proxy)
        {
            string[] split = proxy.Split(':');

            if (split.Length >= 2)
            {
                Host = split[0];

                if (!int.TryParse(split[1], out int port))
                {
                    return;
                }

                Port = port;

                if (split.Length == 4)
                {
                    Credentials = new NetworkCredential(split[2], split[3]);
                }
                else
                {
                    return;
                }
            }

            IsValid = true;
        }

        internal bool IsValid { get; private set; }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public NetworkCredential Credentials { get; set; }

        public ProxySettings Settings { get; set; } = new ProxySettings();

        public HttpClient GetHttpClient()
        {
            var httpMessageHandler = new HttpClientHandler()
            {
                Proxy = new WebProxy(Host, Port)
                {
                    Credentials = Credentials
                }
            };

            var httpClient = new HttpClient(httpMessageHandler);

            if (Settings.Backconnect)
            {
                httpClient.DefaultRequestHeaders.ConnectionClose = true;
            }

            return httpClient;
        }
    }
}
