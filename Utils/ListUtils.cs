using Milky.Objects;
using Milky.Run;
using System;

namespace Milky.Utils
{
    public class ListUtils
    {
        private RunLists _runLists;

        private readonly Random random = new Random();

        public string GetRandomCombo(ComboList combos = null)
        {
            _runLists = RunLists.GetOrNewInstance();

            if (combos == null)
                combos = _runLists.combos;

            return combos[random.Next(combos.Count)];
        }

        public string GetRandomProxy(ProxyList proxies = null)
        {
            _runLists = RunLists.GetOrNewInstance();

            if (proxies == null)
                proxies = _runLists.proxies;

            return proxies[random.Next(proxies.Count)];
        }

        private static ListUtils _classInstance;
        public static ListUtils GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new ListUtils());
        }
    }
}