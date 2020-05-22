namespace Milky.Models
{
    public class CheckerSettings
    {
        /// <param name="maxThreads">Maximum combos to check concurrently</param>
        /// <param name="useProxies">Whether proxies should be used or not</param>
        public CheckerSettings(int maxThreads, bool useProxies)
        {
            MaxThreads = maxThreads;
            UseProxies = useProxies;
        }

        public int MaxThreads { get; private set; }

        public bool UseProxies { get; private set; }
    }
}
