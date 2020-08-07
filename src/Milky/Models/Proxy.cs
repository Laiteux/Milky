using Milky.Enums;
using SocksSharp;
using SocksSharp.Proxy;
using System.Net;
using System.Net.Http;

namespace Milky.Models
{
    public class Proxy
    {
        public Proxy(string proxy, ProxySettings settings)
        {
            Settings = settings;

            string[] split = proxy.Split(':');

            if (split.Length != 2 && split.Length != 4)
            {
                return;
            }

            Host = split[0];

            if (!int.TryParse(split[1], out int port) || port > 65535)
            {
                return;
            }

            Port = port;

            if (split.Length == 4)
            {
                Credentials = new NetworkCredential(split[2], split[3]);
            }

            IsValid = true;
        }

        internal bool IsValid { get; }

        public string Host { get; }

        public int Port { get; }

        public NetworkCredential Credentials { get; }

        public ProxySettings Settings { get; }

        internal HttpClient GetHttpClient()
        {
            var httpMessageHandler = GetHttpMessageHandler();

            var httpClient = new HttpClient(httpMessageHandler);

            if (Settings.Backconnect)
            {
                httpClient.DefaultRequestHeaders.ConnectionClose = true; // MAGIC
            }

            return httpClient;
        }

        private HttpMessageHandler GetHttpMessageHandler()
        {
            if (Settings.Protocol == ProxyProtocol.HTTP)
            {
                return new HttpClientHandler()
                {
                    Proxy = new WebProxy(Host, Port) { Credentials = Credentials },
                    AllowAutoRedirect = Settings.AllowAutoRedirect,
                    UseCookies = Settings.UseCookies,
                    CookieContainer = Settings.CookieContainer ?? new CookieContainer()
                };
            }
            else
            {
                var timeoutMilliseconds = (int)Settings.Timeout.TotalMilliseconds;

                var proxySettings = new SocksSharp.Proxy.ProxySettings()
                {
                    Host = Host, Port = Port,
                    Credentials = Credentials,
                    ConnectTimeout = timeoutMilliseconds,
                    ReadWriteTimeOut = timeoutMilliseconds
                };

                return Settings.Protocol switch
                {
                    ProxyProtocol.SOCKS4 => new ProxyClientHandler<Socks4>(proxySettings)
                    {
                        AllowAutoRedirect = Settings.AllowAutoRedirect,
                        UseCookies = Settings.UseCookies,
                        CookieContainer = Settings.CookieContainer
                    },
                    ProxyProtocol.SOCKS4a => new ProxyClientHandler<Socks4a>(proxySettings)
                    {
                        AllowAutoRedirect = Settings.AllowAutoRedirect,
                        UseCookies = Settings.UseCookies,
                        CookieContainer = Settings.CookieContainer
                    },
                    ProxyProtocol.SOCKS5 => new ProxyClientHandler<Socks5>(proxySettings)
                    {
                        AllowAutoRedirect = Settings.AllowAutoRedirect,
                        UseCookies = Settings.UseCookies,
                        CookieContainer = Settings.CookieContainer
                    }
                };
            }
        }
    }
}
