using Milky.Listeners;
using Milky.Loops;
using Milky.Objects;
using Milky.Statistics;
using Milky.Utils;
using System;
using System.Threading;
using static Milky.Output.OutputSettings;

namespace Milky.Run
{
    public class RunManager
    {
        private ConsoleUtils _consoleUtils;
        private FileUtils _fileUtils;
        private FormatUtils _formatUtils;
        private RunInformations _runInformations;
        private LoopsManager _loopsManager;
        private KeyboardListener _keyboardListener;
        private CustomStatistics _customStatistics;

        public void StartRun()
        {
            _consoleUtils = ConsoleUtils.GetOrNewInstance();
            _runInformations = RunInformations.GetOrNewInstance();
            _loopsManager = LoopsManager.GetOrNewInstance();
            _keyboardListener = KeyboardListener.GetOrNewInstance();

            _runInformations.SetRunStartDate();
            _runInformations.runStatus = RunInformations.RunStatus.Running;

            _loopsManager.StartAllLoops();
            _keyboardListener.StartListening();

            _consoleUtils.WriteLine($"Powered by Milky Library 1.3{Environment.NewLine}");
        }

        public void FinishRun()
        {
            _runInformations = RunInformations.GetOrNewInstance();
            _customStatistics = CustomStatistics.GetOrNewInstance();
            _consoleUtils = ConsoleUtils.GetOrNewInstance();

            _runInformations.runStatus = RunInformations.RunStatus.Finished;

            if(_customStatistics.customStatistics.Count != 0)
            {
                _consoleUtils.WriteLine(null);
                _customStatistics.DisplayCustomStatistics();
            }

            _consoleUtils.UpdateTitle();
            _consoleUtils.Write($"{Environment.NewLine}Finished ");
        }

        public void SubmitComboResult(string combo, ResultType resultType, CaptureDictionary captures = null, bool outputResult = true, string file = null, string directory = null)
        {
            _runInformations = RunInformations.GetOrNewInstance();
            _formatUtils = FormatUtils.GetOrNewInstance();
            _fileUtils = FileUtils.GetOrNewInstance();
            _consoleUtils = ConsoleUtils.GetOrNewInstance();

            if (resultType != ResultType.Invalid)
            {
                Interlocked.Increment(ref _runInformations.hits);

                if(resultType == ResultType.Free)
                    Interlocked.Increment(ref _runInformations.free);

                if (outputResult)
                {
                    string output = _formatUtils.FormatOutput(combo, captures != null ? _formatUtils.CaptureDictionaryToString(captures) : null);

                    _fileUtils.WriteLine(output, $"{file ?? (resultType == ResultType.Free ? "Free" : "Hits")}.txt", directory ?? $"Results/{_runInformations.runStartFormattedDate}");

                    _consoleUtils.WriteLine(output, resultType == ResultType.Free ? ConsoleColor.Cyan : ConsoleColor.Green);
                }
            }

            Interlocked.Increment(ref _runInformations.ran);
        }

        private static RunManager _classInstance;
        public static RunManager GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new RunManager());
        }
    }
}