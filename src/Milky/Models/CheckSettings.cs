namespace Milky.Models
{
    public class CheckSettings
    {
        public int Threads { get; set; }

        public string ResultsFolder { get; set; }

        public bool OutputInConsole { get; set; } = true;

        public bool OutputInvalids { get; set; }
    }
}
