using System;

namespace Milky.Models
{
    public class CheckStatistics
    {
        public DateTime Start { get; internal set; }

        public int Checked { get; internal set; }

        public int Hits { get; internal set; }

        public int Free { get; internal set; }

        public int CPM { get; internal set; }
    }
}
