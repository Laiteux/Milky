using Milky.Models;
using Milky.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Milky
{
    public class CheckerBuilder
    {
        private readonly CheckerSettings _checkerSettings;
        private readonly Func<Combo, HttpClient, Task<CheckResult>> _checkProcess;
        private readonly List<Combo> _combos = new List<Combo>();
        private readonly Library<HttpClient> _httpClientLibrary = new Library<HttpClient>();

        public CheckerBuilder(CheckerSettings checkerSettings, Func<Combo, HttpClient, Task<CheckResult>> checkProcess)
        {
            _checkerSettings = checkerSettings;
            _checkProcess = checkProcess;
        }

        public CheckerBuilder WithCombos(IEnumerable<Combo> combos)
        {
            foreach (var combo in combos.Where(c => c.IsValid))
            {
                _combos.Add(combo);
            }

            return this;
        }

        public CheckerBuilder WithCombos(IEnumerable<string> combos, char separator = ':')
        {
            WithCombos(combos.Select(c => new Combo(c, separator)));

            return this;
        }

        public CheckerBuilder WithProxies(IEnumerable<Proxy> proxies)
        {
            foreach (var proxy in proxies.Where(p => p.IsValid))
            {
                _httpClientLibrary.Add(proxy.GetHttpClient());
            }

            return this;
        }

        public CheckerBuilder WithProxies(IEnumerable<string> proxies, ProxySettings proxySettings = null)
        {
            WithProxies(proxies.Select(p => new Proxy(p)
            {
                Settings = proxySettings
            }));

            return this;
        }

        public Checker Build()
        {
            SetUpMiscellaneous();
            SetUpHttpClientLibrary();

            return new Checker(_checkerSettings, _checkProcess, _combos, _httpClientLibrary);
        }

        private void SetUpMiscellaneous()
        {
            ThreadPool.SetMinThreads(_checkerSettings.MaxThreads + 10, _checkerSettings.MaxThreads + 10);

            Directory.CreateDirectory(_checkerSettings.OutputDirectory);
        }

        private void SetUpHttpClientLibrary()
        {
            if (!_checkerSettings.UseProxies)
            {
                _httpClientLibrary.Items.Clear();

                _httpClientLibrary.Add(new HttpClient(new HttpClientHandler()
                {
                    UseCookies = false
                }));
            }
            else if (_httpClientLibrary.Items.Count == 0)
            {
                throw new Exception("No (valid) proxy loaded");
            }
            else
            {
                _httpClientLibrary.Fill(_checkerSettings.MaxThreads * 2);
            }
        }
    }
}
