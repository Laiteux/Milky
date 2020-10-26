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
        public CheckerInfo Info { get; }

        private readonly CheckerSettings _checkerSettings;
        private readonly OutputSettings _outputSettings;
        private readonly Func<Combo, HttpClient, Task<CheckResult>> _checkProcess;
        private readonly List<Combo> _combos;
        private readonly Library<HttpClient> _httpClientLibrary;

        internal Checker(CheckerSettings checkerSettings, OutputSettings outputSettings, Func<Combo, HttpClient, Task<CheckResult>> checkProcess, List<Combo> combos, Library<HttpClient> httpClientLibrary)
        {
            Info = new CheckerInfo(combos.Count);

            _checkerSettings = checkerSettings;
            _outputSettings = outputSettings;
            _checkProcess = checkProcess;
            _combos = combos;
            _httpClientLibrary = httpClientLibrary;
        }

        public async Task StartAsync()
        {
            if (Info.Status != CheckerStatus.Idle)
            {
                throw new Exception("Checker already started");
            }

            _ = StartCpmCounterAsync();

            Info.Start = DateTime.Now;
            Info.Status = CheckerStatus.Running;

            await _combos.ForEachAsync(_checkerSettings.MaxThreads, async (combo) =>
            {
                if (Info.Status == CheckerStatus.Done)
                {
                    return;
                }

                while (Info.Status == CheckerStatus.Paused)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
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

                OutputCombo(combo, checkResult);

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
            if (Info.Status == CheckerStatus.Idle)
            {
                throw new Exception("Checker not started");
            }
            else if (Info.Status == CheckerStatus.Done)
            {
                throw new Exception("Checker already ended");
            }

            Info.End = DateTime.Now;
            Info.Status = CheckerStatus.Done;
        }

        public void Pause()
        {
            if (Info.Status != CheckerStatus.Running)
            {
                throw new Exception("Checker not running");
            }

            Info.LastPause = DateTime.Now;
            Info.Status = CheckerStatus.Paused;
        }

        /// <returns>Pause duration <see cref="TimeSpan"/></returns>
        public TimeSpan Resume()
        {
            if (Info.Status != CheckerStatus.Paused)
            {
                throw new Exception("Checker not paused");
            }

            TimeSpan pauseDuration = DateTime.Now - Info.LastPause;

            Info.Pause = Info.Pause.Add(pauseDuration);
            Info.Status = CheckerStatus.Running;

            return pauseDuration;
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

        // It just looks better to have a separate method for this
        private void OutputCombo(Combo combo, CheckResult checkResult)
        {
            if (checkResult.ComboResult == ComboResult.Invalid && !_outputSettings.OutputInvalids)
            {
                return;
            }

            var outputBuilder = new StringBuilder(combo.ToString());

            if (checkResult.Captures != null && checkResult.Captures.Count != 0)
            {
                string captures = string.Join(_outputSettings.CaptureSeparator, checkResult.Captures.Where(c => c.Value != null).Select(c => $"{c.Key} = {c.Value}"));

                outputBuilder.Append(_outputSettings.CaptureSeparator).Append(captures);
            }

            var outputString = outputBuilder.ToString();

            string outputPath = Path.Combine(_outputSettings.OutputDirectory ?? string.Empty, checkResult.OutputFile ?? checkResult.ComboResult.ToString()) + ".txt";

            lock (Info.Locker)
            {
                File.AppendAllText(outputPath, outputString + Environment.NewLine);

                Console.ForegroundColor = checkResult.ComboResult switch
                {
                    ComboResult.Hit => _outputSettings.HitColor,
                    ComboResult.Free => _outputSettings.FreeColor,
                    ComboResult.Invalid => _outputSettings.InvalidColor
                };

                Console.WriteLine(outputString);
            }
        }
    }
}
