using System;

namespace Milky.Models
{
    public class CheckStatistics
    {
        /// <summary>
        /// The start <see cref="DateTime"/> of a check
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Count of checked combos
        /// </summary>
        public int Checked { get; set; }

        /// <summary>
        /// Count of <see cref="Enums.CheckResult.Hit"/> combos
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// Count of <see cref="Enums.CheckResult.Free"/> combos
        /// </summary>
        public int Free { get; set; }

        /// <summary>
        /// Checks per minute, updated every 3 seconds from a loop
        /// </summary>
        public int CPM { get; set; }
    }
}
