namespace Milky.Models
{
    public class CheckerSettings
    {
        public CheckerSettings(int maxThreads)
        {
            MaxThreads = maxThreads;
        }

        public int MaxThreads { get; private set; }

        public bool UseProxies { get; set; }

        public string OutputDirectory { get; set; } = "Results";

        public bool OutputInvalids { get; set; }
    }
}
