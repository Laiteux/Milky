using Milky.Enums;
using Milky.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Milky
{
    public class MilkyConsole
    {
        private MilkyCheck _check;
        private Meta _meta;
        private TimeSpan _refreshDelay;

        private bool _running;

        #region Constructors
        public MilkyConsole WithCheck(MilkyCheck check)
        {
            _check = check;

            return this;
        }

        public MilkyConsole WithMeta(Meta meta)
        {
            _meta = meta;

            return this;
        }

        public MilkyConsole WithRefreshDelay(TimeSpan delay)
        {
            _refreshDelay = delay;

            return this;
        }
        #endregion

        public void Start()
        {
            _running = true;

            Task.Run(() =>
            {
                while (_running)
                {
                    var title = new StringBuilder()
                        .Append(_meta.Name + " ")
                        .Append(_meta.Version != null ? $"v{_meta.Version} " : null)
                        .Append(_meta.Author != null ? $"by {_meta.Author} " : null)
                        .Append($"— {_check.Status}");

                    if(_check.Status == CheckStatus.Running || _check.Status == CheckStatus.Paused || _check.Status == CheckStatus.Finished)
                    {
                        var checkStats = new List<string>
                        {
                            "Checked: " + _check.Statistics.Checked,
                            "Hits: " + _check.Statistics.Hits,
                            "Free: " + _check.Statistics.Free
                        };

                        var runStats = new List<string>
                        {
                            "Elapsed: " + TimeSpan.FromSeconds((int)(DateTime.Now - _check.Statistics.Start).TotalSeconds).ToString()
                        };

                        if(_check.Status == CheckStatus.Running || _check.Status == CheckStatus.Paused)
                        {
                            checkStats.Insert(1, "Left: " + (_check.Combos.Count - _check.Statistics.Checked));

                            runStats.Insert(0, "CPM: " + _check.Statistics.CPM);

                            int? estimatedSeconds;

                            try
                            {
                                estimatedSeconds = (_check.Combos.Count - _check.Statistics.Checked) / (_check.Statistics.CPM / 60);
                            }
                            catch
                            {
                                estimatedSeconds = null;
                            }

                            runStats.Insert(2, "Estimated: " + (estimatedSeconds.HasValue ? TimeSpan.FromSeconds(estimatedSeconds.Value).ToString() : "???"));
                        }

                        title
                            .Append(" | ").AppendJoin(" — ", checkStats)
                            .Append(" | ").AppendJoin(" — ", runStats);
                    }

                    Console.Title = title.ToString();

                    if (_check.Status == CheckStatus.Finished)
                        break;

                    Thread.Sleep(_refreshDelay);
                }
            });
        }

        public void Stop()
        {
            _running = false;
        }
    }
}
