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
        private readonly Func<Combo, HttpClient, int, Task<CheckResult>> _checkProcess;
        private OutputSettings _outputSettings = new OutputSettings();
        private readonly List<Combo> _combos = new List<Combo>();
        private readonly Library<HttpClient> _httpClientLibrary = new Library<HttpClient>();
        private readonly Dictionary<string, string> _defaultRequestHeaders = new Dictionary<string, string>();

        public CheckerBuilder(CheckerSettings checkerSettings, Func<Combo, HttpClient, int, Task<CheckResult>> checkProcess)
        {
            _checkerSettings = checkerSettings;
            _checkProcess = checkProcess;
        }

        public CheckerBuilder WithOutputSettings(OutputSettings outputSettings)
        {
            _outputSettings = outputSettings;

            return this;
        }

        public CheckerBuilder WithCombos(IEnumerable<Combo> combos)
        {
            foreach (var combo in combos.Where(c => c.Valid))
            {
                _combos.Add(combo);
            }

            return this;
        }

        public CheckerBuilder WithCombos(IEnumerable<string> combos, string separator = ":")
        {
            WithCombos(combos.Select(c => new Combo(c, separator)));

            return this;
        }

        public CheckerBuilder WithProxies(IEnumerable<Proxy> proxies)
        {
            foreach (var proxy in proxies.Where(p => p.Valid))
            {
                _httpClientLibrary.Add(proxy.GetHttpClient());
            }

            return this;
        }

        public CheckerBuilder WithProxies(IEnumerable<string> proxies, ProxySettings settings)
        {
            WithProxies(proxies.Select(p => new Proxy(p, settings)));

            return this;
        }

        public CheckerBuilder WithDefaultRequestHeader(string name, string value)
        {
            _defaultRequestHeaders.Add(name, value);

            return this;
        }

        public CheckerBuilder WithDefaultRequestHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                WithDefaultRequestHeader(header.Key, header.Value);
            }

            return this;
        }

        public Checker Build()
        {
            SetUpHttpClientLibrary();
            SetUpMiscellaneous();

            return new Checker(_checkerSettings, _outputSettings, _checkProcess, _combos, _httpClientLibrary);
        }

        private void SetUpMiscellaneous(int extraThreads = 10)
        {
            ThreadPool.SetMinThreads(_checkerSettings.MaxThreads + extraThreads, _checkerSettings.MaxThreads + extraThreads);

            Directory.CreateDirectory(_outputSettings.OutputDirectory);
        }

        private void SetUpHttpClientLibrary()
        {
            if (!_checkerSettings.UseProxies)
            {
                _httpClientLibrary.Items.Clear(); // Just making sure

                _httpClientLibrary.Add(new HttpClient(new HttpClientHandler()
                {
                    UseCookies = false // Using cookies would suck with shared HttpClients, especially for credential stuffing
                }));
            }
            else if (_httpClientLibrary.Items.Count == 0)
            {
                throw new Exception("No (valid) proxy loaded.");
            }
            else
            {
                _httpClientLibrary.Fill(_checkerSettings.MaxThreads * 2); // Lazy to explain, use your brain
            }

            foreach (var header in _defaultRequestHeaders)
            {
                foreach (var httpClient in _httpClientLibrary.Items)
                {
                    httpClient.Value.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }
    }
}
