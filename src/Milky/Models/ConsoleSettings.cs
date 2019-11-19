namespace Milky.Models
{
    public class ConsoleSettings
    {
        /// <summary>
        /// Whether or not you'd like the <see cref="Enums.CheckResult.Free"/> combos count to be displayed in the console title
        /// </summary>
        public bool ShowFree { get; set; }

        /// <summary>
        /// Whether or not you'd like percentages to be displayed next to statistics in the console title
        /// </summary>
        public bool ShowPercentages { get; set; }
    }
}
