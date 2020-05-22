using Milky.Enums;
using System;

namespace Milky.Models
{
    // Might not seem so good at the first look but perfectly does the job
    internal class CheckerInfo
    {
        internal CheckerInfo(int combos)
        {
            Combos = combos;
        }

        internal object Locker { get; } = new object();

        internal CheckerStatus Status { get; set; }

        internal int Combos { get; private set; }

        internal int Checked { get; set; }

        internal int Cpm { get; set; }

        internal int Hits { get; set; }

        internal int Free { get; set; }

        internal int Retries { get; set; }

        internal int EstimatedHits
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

        internal DateTime Start { get; set; }

        internal DateTime? End { get; set; }

        internal DateTime LastPause { get; set; }

        internal TimeSpan Pause { get; set; } = new TimeSpan();

        internal TimeSpan Elapsed => TimeSpan.FromSeconds((int)((End ?? DateTime.Now) - Start - Pause - (Status == CheckerStatus.Paused ? (DateTime.Now - LastPause) : TimeSpan.Zero)).TotalSeconds);

        internal TimeSpan? Remaining
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
