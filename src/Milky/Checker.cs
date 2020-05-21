using Milky.Enums;
using Milky.Extensions;
using Milky.Models;
using Milky.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Milky
{
    public class Checker
    {
        internal CheckerInfo Info { get; private set; }

        private readonly CheckerSettings _checkerSettings;
        private readonly Func<Combo, HttpClient, Task<CheckResult>> _checkProcess;
        private readonly List<Combo> _combos;
        private readonly Library<HttpClient> _httpClientLibrary;

        internal Checker(CheckerSettings checkerSettings, Func<Combo, HttpClient, Task<CheckResult>> checkProcess, List<Combo> combos, Library<HttpClient> httpClientLibrary)
        {
            Info = new CheckerInfo(combos.Count);

            _checkerSettings = checkerSettings;
            _checkProcess = checkProcess;
            _combos = combos;
            _httpClientLibrary = httpClientLibrary;
        }

        public async Task StartAsync()
        {
            _ = StartCpmCounterAsync();

            Info.Start = DateTime.Now;
            Info.Status = CheckerStatus.Running;

            await _combos.ForEachAsync(_checkerSettings.MaxThreads, async (combo) =>
            {
                while (Info.Status == CheckerStatus.Paused)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                if (Info.Status == CheckerStatus.Done)
                {
                    return;
                }

                KeyValuePair<int, HttpClient> httpClient;
                CheckResult checkResult;

                for (; ; )
                {
                    if (_checkerSettings.UseProxies)
                    {
                        _httpClientLibrary.TryBorrowRandom(out httpClient);
                    }
                    else
                    {
                        httpClient = _httpClientLibrary.Items[0];
                    }

                    checkResult = await _checkProcess(combo, httpClient.Value).ConfigureAwait(false);

                    _httpClientLibrary.Return(httpClient);

                    if (checkResult.ComboResult == ComboResult.Retry)
                    {
                        lock (Info.Locker)
                        {
                            Info.Retries++;
                        }

                        continue;
                    }

                    break;
                }

                if (checkResult.ComboResult == ComboResult.Invalid && _checkerSettings.OutputInvalids)
                {
                    OutputCombo(combo, checkResult);
                }

                lock (Info.Locker)
                {
                    Info.Checked++;

                    if (checkResult.ComboResult == ComboResult.Hit)
                    {
                        Info.Hits++;
                    }
                    else if (checkResult.ComboResult == ComboResult.Free)
                    {
                        Info.Free++;
                    }
                }
            }).ConfigureAwait(false);

            if (Info.Status != CheckerStatus.Done)
            {
                End();
            }
        }

        public void End()
        {
            Info.End = DateTime.Now;
            Info.Status = CheckerStatus.Done;
        }

        public void Pause()
        {
            Info.LastPause = DateTime.Now;
            Info.Status = CheckerStatus.Paused;
        }

        public TimeSpan Resume()
        {
            if (Info.Status != CheckerStatus.Paused)
            {
                throw new Exception("Checker isn't paused");
            }

            var resumed = DateTime.Now;

            Info.Pause = Info.Pause.Add(resumed - Info.LastPause);
            Info.Status = CheckerStatus.Running;

            return resumed - Info.LastPause;
        }

        private async Task StartCpmCounterAsync()
        {
            while (Info.Status != CheckerStatus.Done)
            {
                int checkedBefore = Info.Checked;
                await Task.Delay(6000).ConfigureAwait(false);
                int checkedAfter = Info.Checked;

                Info.Cpm = (checkedAfter - checkedBefore) * 10;
            }
        }

        private void OutputCombo(Combo combo, CheckResult checkResult)
        {
            var outputBuilder = new StringBuilder(combo.ToString());

            if (checkResult.Captures.Count != 0)
            {
                string captures = string.Join(" | ", checkResult.Captures.Select(c => $"{c.Key} = {c.Value}"));

                outputBuilder.Append(" | ").Append(captures);
            }

            var outputString = outputBuilder.ToString();

            string outputPath = Path.Combine(_checkerSettings.OutputDirectory, checkResult.OutputFile ?? checkResult.ComboResult.ToString()) + ".txt";

            lock (Info.Locker)
            {
                File.AppendAllText(outputPath, outputString + Environment.NewLine);

                Console.ForegroundColor = checkResult.ComboResult switch
                {
                    ComboResult.Hit => ConsoleColor.Green,
                    ComboResult.Free => ConsoleColor.Cyan,
                    ComboResult.Invalid => ConsoleColor.Red
                };

                Console.WriteLine(outputString);
            }
        }
    }
}
