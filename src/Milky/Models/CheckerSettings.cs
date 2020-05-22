namespace Milky.Models
{
    public class CheckerSettings
    {
        public CheckerSettings(int maxThreads, bool useProxies)
        {
            MaxThreads = maxThreads;
            UseProxies = useProxies;
        }

        public int MaxThreads { get; private set; }

        public bool UseProxies { get; private set; }
    }
}
