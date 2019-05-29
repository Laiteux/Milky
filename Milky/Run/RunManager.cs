using Milky.Loops;
using Milky.Objects;
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

        public void StartRun()
        {
            _consoleUtils = ConsoleUtils.GetOrNewInstance();
            _fileUtils = FileUtils.GetOrNewInstance();
            _formatUtils = FormatUtils.GetOrNewInstance();
            _runInformations = RunInformations.GetOrNewInstance();
            _loopsManager = LoopsManager.GetOrNewInstance();

            _runInformations.SetRunStartDate();
            _runInformations.runStatus = RunInformations.RunStatus.Running;

            _loopsManager.StartAllLoops();

            _consoleUtils.WriteLine($"Powered by Milky Library 1.1{Environment.NewLine}");
        }

        public void FinishRun()
        {
            _runInformations.runStatus = RunInformations.RunStatus.Finished;

            _consoleUtils.UpdateTitle();

            _consoleUtils.Write($"{Environment.NewLine}Finished ");
        }

        public void SubmitComboResult(string combo, ResultType resultType, CaptureDictionary captures = null, bool outputResult = true, string file = null, string directory = null)
        {
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