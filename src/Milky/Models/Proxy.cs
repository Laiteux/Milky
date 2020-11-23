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

            Valid = true;
        }

        internal bool Valid { get; }

        internal ProxySettings Settings { get; }

        internal string Host { get; }

        internal int Port { get; }

        internal NetworkCredential Credentials { get; }

        internal HttpClient GetHttpClient()
        {
            var httpMessageHandler = GetHttpMessageHandler();

            var httpClient = new HttpClient(httpMessageHandler)
            {
                Timeout = Settings.Timeout
            };

            if (Settings.Backconnect)
            {
                httpClient.DefaultRequestHeaders.ConnectionClose = true; // MAGIC
            }

            return httpClient;
        }

        private HttpMessageHandler GetHttpMessageHandler()
        {
            if (Settings.Protocol == ProxyProtocol.Http)
            {
                return new HttpClientHandler()
                {
                    Proxy = new WebProxy(Host, Port) { Credentials = Credentials },
                    AllowAutoRedirect = Settings.AllowAutoRedirect,
                    UseCookies = Settings.UseCookies,
                    CookieContainer = Settings.CookieContainer ?? new CookieContainer()
                };
            }

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
                ProxyProtocol.Socks4 => new ProxyClientHandler<Socks4>(proxySettings)
                {
                    AllowAutoRedirect = Settings.AllowAutoRedirect,
                    UseCookies = Settings.UseCookies,
                    CookieContainer = Settings.CookieContainer
                },
                ProxyProtocol.Socks4A => new ProxyClientHandler<Socks4a>(proxySettings)
                {
                    AllowAutoRedirect = Settings.AllowAutoRedirect,
                    UseCookies = Settings.UseCookies,
                    CookieContainer = Settings.CookieContainer
                },
                ProxyProtocol.Socks5 => new ProxyClientHandler<Socks5>(proxySettings)
                {
                    AllowAutoRedirect = Settings.AllowAutoRedirect,
                    UseCookies = Settings.UseCookies,
                    CookieContainer = Settings.CookieContainer
                }
            };
        }
    }
}
