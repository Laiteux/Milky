namespace Milky.Settings
{
    public class RunSettings
    {
        public int
            threads = 0,
            proxyTimeout = 5000;

        public string proxyProtocol = null;

        private static RunSettings _classInstance;
        public static RunSettings GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new RunSettings());
        }
    }
}