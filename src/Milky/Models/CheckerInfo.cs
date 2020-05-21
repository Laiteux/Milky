using Milky.Enums;
using System;

namespace Milky.Models
{
    internal class CheckerInfo
    {
        public CheckerInfo(int combos)
        {
            Combos = combos;
        }

        internal object Locker => new object();

        public CheckerStatus Status { get; internal set; }

        internal int Combos { get; private set; }

        public int Checked { get; internal set; }

        public int Cpm { get; internal set; }

        public int Hits { get; internal set; }

        public int Free { get; internal set; }

        public int Retries { get; internal set; }

        public int EstimatedHits
        {
            get
            {
                if (Checked == 0)
                {
                    return 0;
                }

                return (int)((double)Combos / Checked * Hits);
            }
        }

        public DateTime Start { get; internal set; }

        public DateTime? End { get; internal set; }

        public DateTime LastPause { get; internal set; }

        internal TimeSpan Pause { get; set; } = new TimeSpan();

        public TimeSpan Elapsed => TimeSpan.FromSeconds((int)((End ?? DateTime.Now) - Start - Pause - (Status == CheckerStatus.Paused ? (DateTime.Now - LastPause) : TimeSpan.Zero)).TotalSeconds);

        public TimeSpan? Remaining
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds((Combos - Checked) / (Cpm / 60));
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
