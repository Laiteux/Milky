using Milky.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Milky
{
    public class ConsoleManager
    {
        private readonly Checker _checker;

        public ConsoleManager(Checker checker)
        {
            _checker = checker;
        }

        public async Task StartUpdatingTitleAsync(TimeSpan updateInterval, bool showFree = true, bool showPercentages = true, string prefix = null, string suffix = null)
        {
            while (true)
            {
                var title = new StringBuilder(prefix).Append(_checker.Info.Status);

                if (_checker.Info.Status != CheckerStatus.Idle)
                {
                    var checkStats = new List<string>
                    {
                        "Checked: " + ((double)_checker.Info.Checked).ToString("N0"),
                        "Hits: " + ((double)_checker.Info.Hits).ToString("N0")
                    };

                    if (showFree)
                    {
                        checkStats.Add("Free: " + ((double)_checker.Info.Free).ToString("N0"));
                    }

                    if (showPercentages)
                    {
                        if (_checker.Info.Status != CheckerStatus.Done)
                        {
                            checkStats[0] += $" ({(double)_checker.Info.Checked / _checker.Info.Combos:P2})";
                        }

                        checkStats[1] += $" ({(double)_checker.Info.Hits / _checker.Info.Checked:P2})";

                        if (showFree)
                        {
                            checkStats[2] += $" ({(double)_checker.Info.Free / _checker.Info.Checked:P2})";
                        }
                    }

                    var runStats = new List<string>
                    {
                        "Elapsed: " + _checker.Info.Elapsed.ToString()
                    };

                    if (_checker.Info.Status != CheckerStatus.Done)
                    {
                        checkStats.Insert(1, "Left: " + ((double)(_checker.Info.Combos - _checker.Info.Checked)).ToString("N0"));

                        if (_checker.Info.Hits != 0)
                        {
                            checkStats.Insert(3, "Estimated: " + _checker.Info.EstimatedHits.ToString("N0"));
                        }

                        if (_checker.Info.Status != CheckerStatus.Paused)
                        {
                            runStats.Insert(0, "CPM: " + ((double)_checker.Info.Cpm).ToString("N0"));
                            runStats.Insert(2, "Remaining: " + (_checker.Info.Remaining?.ToString() ?? "?"));
                        }
                    }

                    title
                        .Append(" | ").Append(string.Join(" — ", checkStats))
                        .Append(" | ").Append(string.Join(" — ", runStats));
                }

                Console.Title = title.Append(suffix).ToString();

                if (_checker.Info.Status == CheckerStatus.Done)
                {
                    break;
                }

                await Task.Delay(updateInterval).ConfigureAwait(false);
            }
        }

        public async Task StartListeningKeysAsync(ConsoleKey? pauseKey = ConsoleKey.P, ConsoleKey? resumeKey = ConsoleKey.R, ConsoleKey? endKey = null)
        {
            while (_checker.Info.Status != CheckerStatus.Done)
            {
                if (!Console.KeyAvailable)
                {
                    await Task.Delay(100).ConfigureAwait(false);

                    continue;
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                if ((pauseKey != null && key == pauseKey) || (resumeKey != null && key == resumeKey))
                {
                    if (_checker.Info.Status == CheckerStatus.Running)
                    {
                        _checker.Pause();

                        lock (_checker.Info.Locker)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"{Environment.NewLine} Checker paused, press \"{resumeKey}\" to resume...{Environment.NewLine}");
                        }
                    }
                    else if (_checker.Info.Status == CheckerStatus.Paused)
                    {
                        lock (_checker.Info.Locker)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"Checker resumed! Pause duration: {TimeSpan.FromSeconds((int)_checker.Resume().TotalSeconds)}" + Environment.NewLine);
                        }
                    }
                }
                else if (endKey != null && key == endKey)
                {
                    _checker.End();
                }
            }
        }
    }
}
