using Milky.Enums;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Milky.Models
{
    public class CheckerInfo
    {
        internal CheckerInfo(int combos)
        {
            Combos = combos;
        }

        internal object Locker { get; } = new object();

        internal CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        public CheckerStatus Status { get; internal set; }

        public int Combos { get; }

        public List<Combo> Checked { get; } = new List<Combo>();

        public int Cpm { get; internal set; }

        public int Hits { get; internal set; }

        public int Free { get; internal set; }

        public int Retries { get; internal set; }

        public int EstimatedHits
        {
            get
            {
                if (Checked.Count == 0 || Hits == 0)
                {
                    return 0;
                }

                return (int)((double)Combos / Checked.Count * Hits);
            }
        }

        public DateTime Start { get; internal set; }

        public DateTime? End { get; internal set; }

        internal DateTime LastPause { get; set; }

        internal TimeSpan TotalPause { get; set; }

        internal DateTime LastHit { get; set; }

        public TimeSpan Elapsed => TimeSpan.FromSeconds((int)((End ?? DateTime.Now) - Start - TotalPause - (Status == CheckerStatus.Paused ? DateTime.Now - LastPause : TimeSpan.Zero)).TotalSeconds);

        public TimeSpan? Remaining
        {
            get
            {
                try
                {
                    return TimeSpan.FromSeconds((Combos - Checked.Count) / (Cpm / 60));
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
