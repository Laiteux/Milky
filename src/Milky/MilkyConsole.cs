using Milky.Enums;
using Milky.Extensions;
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
        private ConsoleSettings _settings;
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

        public MilkyConsole WithSettings(ConsoleSettings settings)
        {
            _settings = settings;

            return this;
        }

        public MilkyConsole WithRefreshDelay(TimeSpan delay)
        {
            _refreshDelay = delay;

            return this;
        }
        #endregion

        public async Task StartAsync()
        {
            _running = true;

            await Task.Run(() =>
            {
                while (_running)
                {
                    var title = new StringBuilder()
                        .Append(_meta.Name + " ")
                        .Append(_meta.Version != null ? $"{_meta.Version} " : null)
                        .Append(_meta.Author != null ? $"by {_meta.Author} " : null)
                        .Append($"— {_check.Status}");

                    if (_check.Status != CheckStatus.Idle)
                    {
                        var checkStats = new List<string>
                        {
                            "Checked: " + ((double)_check.Statistics.Checked).FormatInvariantCulture("n0"),
                            "Hits: " + ((double)_check.Statistics.Hits).FormatInvariantCulture("n0")
                        };

                        if (_settings.ShowFree)
                        {
                            checkStats.Add("Free: " + ((double)_check.Statistics.Free).FormatInvariantCulture("n0"));
                        }

                        if (_settings.ShowPercentages)
                        {
                            if(_check.Status != CheckStatus.Finished)
                            {
                                checkStats[0] += $" ({((double)_check.Statistics.Checked / _check.Combos.Count).FormatInvariantCulture("P")})";
                            }

                            checkStats[1] += $" ({((double)_check.Statistics.Hits / _check.Statistics.Checked).FormatInvariantCulture("P")})";

                            if (_settings.ShowFree)
                            {
                                checkStats[2] += $" ({((double)_check.Statistics.Free / _check.Statistics.Checked).FormatInvariantCulture("P")})";
                            }
                        }

                        var runStats = new List<string>
                        {
                            "Elapsed: " + TimeSpan.FromSeconds((int)(DateTime.Now - _check.Statistics.Start).TotalSeconds).ToString()
                        };

                        if (_check.Status != CheckStatus.Finished)
                        {
                            checkStats.Insert(1, "Left: " + ((double)(_check.Combos.Count - _check.Statistics.Checked)).FormatInvariantCulture("n0"));

                            runStats.Insert(0, "CPM: " + ((double)_check.Statistics.CPM).FormatInvariantCulture("n0"));

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
                    {
                        break;
                    }

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
