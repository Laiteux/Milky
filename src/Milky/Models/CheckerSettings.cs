namespace Milky.Models
{
    public class CheckerSettings
    {
        /// <param name="maxThreads">Maximum combos to check concurrently</param>
        /// <param name="useProxies">Whether proxies should be used or not</param>
        /// <param name="useCookies">Whether or not to use/save/keep cookies, usually not recommended for credential stuffing but who knows</param>
        /// <param name="AllowAutoRedirect">Whether or not to follow redirections</param>
        /// <param name="MaxAutomaticRedirections">Max automatic request redirections</param>
        public CheckerSettings(int maxThreads, bool useProxies, bool useCookies = false, bool allowAutoRedirect = true, int maxAutomaticRedirections = 50)
        {
            MaxThreads = maxThreads;
            UseProxies = useProxies;
            UseCookies = useCookies;
            AllowAutoRedirect = allowAutoRedirect;
            MaxAutomaticRedirections = maxAutomaticRedirections;
        }

        public int MaxThreads { get; }
        public bool UseProxies { get; }
        public bool UseCookies { get; }
        public bool AllowAutoRedirect { get; set; }
        public int MaxAutomaticRedirections { get; set; }
    }
}