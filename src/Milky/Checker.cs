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
        private readonly Func<Combo, HttpClient, int, Task<CheckResult>> _checkProcess;
        private readonly List<Combo> _combos;
        private readonly Library<HttpClient> _httpClientLibrary;

        internal Checker(CheckerSettings checkerSettings, OutputSettings outputSettings, Func<Combo, HttpClient, int, Task<CheckResult>> checkProcess, List<Combo> combos, Library<HttpClient> httpClientLibrary)
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
                throw new Exception("Checker already started.");
            }

            _ = StartCpmCounterAsync();

            Info.Start = DateTime.Now;
            Info.Status = CheckerStatus.Running;

            await _combos.ForEachAsync(_checkerSettings.MaxThreads, async combo =>
            {
                Info.CancellationTokenSource.Token.ThrowIfCancellationRequested();

                while (Info.Status == CheckerStatus.Paused)
                {
                    await Task.Delay(1000).ConfigureAwait(false); // I'm not sure if this is the best practice
                }

                int attempts = 1;
                CheckResult checkResult;

                while (true)
                {
                    KeyValuePair<int, HttpClient> httpClient;

                    if (_checkerSettings.UseProxies)
                    {
                        _httpClientLibrary.TryBorrowRandom(out httpClient);
                    }
                    else
                    {
                        httpClient = _httpClientLibrary.Items[0];
                    }

                    checkResult = await _checkProcess(combo, httpClient.Value, attempts).ConfigureAwait(false);

                    _httpClientLibrary.Return(httpClient);

                    if (checkResult.IncrementAttempts)
                    {
                        attempts++;
                    }

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
                    Info.Checked.Add(combo);

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

            lock (Info.Locker)
            {
                if (!Info.CancellationTokenSource.IsCancellationRequested)
                {
                    Abort();
                }
            }
        }

        public void Abort()
        {
            lock (Info.Locker)
            {
                if (Info.Status == CheckerStatus.Idle)
                {
                    throw new Exception("Checker not started.");
                }

                if (Info.Status == CheckerStatus.Done)
                {
                    throw new Exception("Checker already ended.");
                }

                Info.CancellationTokenSource.Cancel();
                Info.End = DateTime.Now;
                Info.Status = CheckerStatus.Done;
            }
        }

        public void Pause()
        {
            lock (Info.Locker)
            {
                if (Info.Status != CheckerStatus.Running)
                {
                    throw new Exception("Checker not running.");
                }

                Info.LastPause = DateTime.Now;
                Info.Status = CheckerStatus.Paused;
            }
        }

        /// <returns>Pause duration <see cref="TimeSpan"/></returns>
        public TimeSpan Resume()
        {
            lock (Info.Locker)
            {
                if (Info.Status != CheckerStatus.Paused)
                {
                    throw new Exception("Checker not paused.");
                }

                TimeSpan pauseDuration = DateTime.Now - Info.LastPause;

                Info.TotalPause = Info.TotalPause.Add(pauseDuration);
                Info.Status = CheckerStatus.Running;

                return pauseDuration;
            }
        }

        public int SaveUnchecked()
        {
            lock (Info.Locker)
            {
                Pause();

                string outputPath = Path.Combine(_outputSettings.OutputDirectory ?? string.Empty, "Unchecked.txt");

                List<Combo> @unchecked = _combos.Except(Info.Checked).ToList();

                File.WriteAllLines(outputPath, @unchecked.Select(c => c.ToString()));

                Resume();

                return @unchecked.Count;
            }
        }

        private async Task StartCpmCounterAsync()
        {
            while (Info.Status != CheckerStatus.Done)
            {
                int checkedBefore = Info.Checked.Count;
                await Task.Delay(6000).ConfigureAwait(false);
                int checkedAfter = Info.Checked.Count;

                Info.Cpm = (checkedAfter - checkedBefore) * 10;
            }
        }

        private void OutputCombo(Combo combo, CheckResult checkResult)
        {
            if (checkResult.ComboResult == ComboResult.Invalid && !_outputSettings.OutputInvalids)
            {
                return;
            }

            var outputBuilder = new StringBuilder(combo.ToString());

            if (checkResult.Captures != null && checkResult.Captures.Count != 0)
            {
                IEnumerable<string> captures = checkResult.Captures
                    .Where(c => !string.IsNullOrWhiteSpace(c.Value?.ToString())) // If capture value is either null, empty or white-space, we don't want it to be included
                    .Select(c => $"{c.Key} = {c.Value}");

                outputBuilder.Append(_outputSettings.CaptureSeparator).AppendJoin(_outputSettings.CaptureSeparator, captures);
            }

            var outputString = outputBuilder.ToString();

            lock (Info.Locker)
            {
                foreach (string outputFile in checkResult.OutputFiles ?? new[] { checkResult.ComboResult.ToString() })
                {
                    string outputPath = Path.Combine(_outputSettings.OutputDirectory ?? string.Empty, outputFile + ".txt");

                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                    File.AppendAllText(outputPath, outputString + Environment.NewLine);

                    if (_outputSettings.GlobalOutput)
                    {
                        string globalOutputPath = Path.Combine(Path.Combine(_outputSettings.OutputDirectory != null ? Directory.GetParent(_outputSettings.OutputDirectory).FullName : "Results", "Global"), outputFile + ".txt");

                        Directory.CreateDirectory(Path.GetDirectoryName(globalOutputPath));

                        File.AppendAllText(globalOutputPath, outputString + Environment.NewLine);
                    }

                    Console.ForegroundColor = checkResult.ComboResult switch
                    {
                        ComboResult.Hit => _outputSettings.HitColor,
                        ComboResult.Free => _outputSettings.FreeColor,
                        ComboResult.Invalid => _outputSettings.InvalidColor
                    };

                    if (_outputSettings.SpecialColors.Count > 0 && checkResult.Captures != null)
                    {
                        foreach (var specialColor in _outputSettings.SpecialColors)
                        {
                            if (checkResult.Captures.TryGetValue(specialColor.Value.Key, out var capturedObject))
                            {
                                if (specialColor.Value.Value(capturedObject))
                                {
                                    Console.ForegroundColor = specialColor.Key;
                                    break;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine(outputString);
                Info.LastHit = DateTime.Now;
            }
        }
    }
}
