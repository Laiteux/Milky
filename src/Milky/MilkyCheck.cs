using Milky.Enums;
using Milky.Extensions;
using Milky.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Milky
{
    public class MilkyCheck
    {
        public CheckStatistics Statistics = new CheckStatistics();
        public CheckStatus Status = CheckStatus.Idle;

        private readonly object _randomLocker = new object();
        private readonly object _statisticsLocker = new object();
        private readonly object _outputLocker = new object();

        internal ICollection<Combo> Combos;
        internal ICollection<string> Proxies;
        internal CheckSettings Settings;
        internal Func<Combo, string, Task<(CheckResult, ICollection<KeyValuePair<string, string>>, bool)>> CheckingProcess;

        #region Constructors
        public MilkyCheck WithCombos(ICollection<Combo> combos)
        {
            Combos = combos;

            return this;
        }

        public MilkyCheck WithProxies(ICollection<string> proxies)
        {
            Proxies = proxies;

            return this;
        }

        public MilkyCheck WithSettings(CheckSettings settings)
        {
            Settings = settings;

            ThreadPool.SetMinThreads(Settings.Threads, Settings.Threads);

            return this;
        }

        public MilkyCheck WithCheckingProcess(Func<Combo, string, Task<(CheckResult Result, ICollection<KeyValuePair<string, string>> Captures, bool NewProxy)>> process)
        {
            CheckingProcess = process;

            return this;
        }
        #endregion

        public async Task StartAsync()
        {
            Status = CheckStatus.Running;
            Statistics.Start = DateTime.Now;

            _ = StartCpmCounterAsync();

            var random = new Random();
            bool proxyless = Proxies == null;

            Settings.ResultsFolder ??= "Results/" + CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Statistics.Start.ToString("MMM dd, yyyy — HH.mm.ss"));
            Directory.CreateDirectory(Settings.ResultsFolder);

            await Combos.ForEachAsync(Settings.Threads, async combo =>
            {
                while (Status == CheckStatus.Paused)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                if (Status == CheckStatus.Running)
                {
                    string proxy = null;

                    if (!proxyless)
                    {
                        lock (_randomLocker)
                        {
                            proxy = Proxies.GetRandomItem(random);
                        }
                    }

                    while (true)
                    {
                        var (result, captures, newProxy) = await CheckingProcess(combo, proxy);

                        if (result == CheckResult.Retry)
                        {
                            if (newProxy && !proxyless)
                            {
                                lock (_randomLocker)
                                {
                                    proxy = Proxies.GetRandomItem(random);
                                }
                            }

                            continue;
                        }

                        lock (_statisticsLocker)
                        {
                            Statistics.Checked++;

                            if (result == CheckResult.Hit)
                            {
                                Statistics.Hits++;
                            }
                            else if (result == CheckResult.Free)
                            {
                                Statistics.Free++;
                            }
                        }

                        if (result != CheckResult.Unknown)
                        {
                            if (result == CheckResult.Invalid && !Settings.OutputInvalids)
                            {
                                break;
                            }

                            var output = new StringBuilder(combo.ToString());

                            if (captures != null && captures.Count != 0)
                            {
                                output.Append(" | ");
                                output.Append(string.Join(" | ", captures.Select(c => $"{c.Key} = {c.Value}")));
                            }

                            lock (_outputLocker)
                            {
                                File.AppendAllText($"{Settings.ResultsFolder}/{result.ToString()}.txt", output + Environment.NewLine);

                                if (Settings.OutputInConsole)
                                {
                                    Console.ForegroundColor = result switch
                                    {
                                        CheckResult.Hit => ConsoleColor.Green,
                                        CheckResult.Free => ConsoleColor.Cyan,
                                        CheckResult.Invalid => ConsoleColor.Red
                                    };

                                    Console.WriteLine(output);
                                }
                            }
                        }

                        break;
                    }
                }
            });

            Status = CheckStatus.Finished;
        }

        public void Stop() => Status = CheckStatus.Finished;
        public void Pause() => Status = CheckStatus.Paused;
        public void Resume() => Status = CheckStatus.Running;

        private async Task StartCpmCounterAsync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (Status == CheckStatus.Finished)
                    {
                        break;
                    }

                    int checkedBefore = Statistics.Checked;
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    int checkedAfter = Statistics.Checked;

                    Statistics.CPM = (checkedAfter - checkedBefore) * 20;
                }
            });
        }
    }
}
