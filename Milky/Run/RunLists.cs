using Milky.Objects;

namespace Milky.Run
{
    public class RunLists
    {
        public ComboList combos = new ComboList();
        public ProxyList proxies = new ProxyList();

        private static RunLists _classInstance;
        public static RunLists GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new RunLists());
        }
    }
}