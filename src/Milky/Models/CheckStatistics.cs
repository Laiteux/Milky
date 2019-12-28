using System;

namespace Milky.Models
{
    public class CheckStatistics
    {
        public DateTime Start { get; set; }

        public int Checked { get; set; }

        public int Hits { get; set; }

        public int Free { get; set; }

        public int CPM { get; set; }
    }
}
