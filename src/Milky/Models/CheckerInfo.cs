using Milky.Enums;
using System;
using System.Threading;

namespace Milky.Models
{
    // MIGHT not seem so good at the first look but perfectly does the job
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

        public int Checked { get; internal set; }

        public int Cpm { get; internal set; }

        public int Hits { get; internal set; }

        public int Free { get; internal set; }

        public int Retries { get; internal set; }

        public int EstimatedHits
        {
            get
            {
                if (Checked == 0 || Hits == 0)
                {
                    return 0;
                }

                return (int)((double)Combos / Checked * Hits);
            }
        }

        public DateTime Start { get; internal set; }

        public DateTime? End { get; internal set; }

        internal DateTime LastPause { get; set; }

        internal TimeSpan TotalPause { get; set; }

        public TimeSpan Elapsed => TimeSpan.FromSeconds((int)((End ?? DateTime.Now) - Start - TotalPause - (Status == CheckerStatus.Paused ? DateTime.Now - LastPause : TimeSpan.Zero)).TotalSeconds);

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
