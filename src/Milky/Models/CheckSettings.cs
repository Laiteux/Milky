namespace Milky.Models
{
    public class CheckSettings
    {
        /// <summary>
        /// Maximum simultaneous accounts to be checked
        /// </summary>
        public int Threads { get; set; }

        /// <summary>
        /// Whether or not you'd like to output combos results in console
        /// </summary>
        public bool OutputInConsole { get; set; } = true;

        /// <summary>
        /// Whether or not you'd like to output <see cref="Enums.CheckResult.Invalid"/> combos (console, results file), setting this to true can considerably decrease your check speed
        /// </summary>
        public bool OutputInvalids { get; set; }
    }
}
