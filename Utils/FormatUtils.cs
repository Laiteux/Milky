using Milky.Objects;
using Milky.Output;
using Milky.Program;
using Milky.Run;
using Milky.Statistics;
using System.Collections.Generic;

namespace Milky.Utils
{
    public class FormatUtils
    {
        private ProgramInformations _programInformations;
        private RunInformations _runInformations;
        private RunLists _runLists;
        private RunStatistics _runStatistics;
        private CustomStatistics _customStatistics;
        private OutputSettings _outputSettings;

        public string FormatTitle(string text)
        {
            _programInformations = ProgramInformations.GetOrNewInstance();
            _runInformations = RunInformations.GetOrNewInstance();
            _runLists = RunLists.GetOrNewInstance();
            _runStatistics = RunStatistics.GetOrNewInstance();
            _customStatistics = CustomStatistics.GetOrNewInstance();
            _outputSettings = OutputSettings.GetOrNewInstance();

            text = text
                .Replace("%program.name%", _programInformations._name)
                .Replace("%program.version%", _programInformations._version)
                .Replace("%program.author%", _programInformations._author)

                .Replace("%lists.combos%", _runLists.combos.Count.ToString())
                .Replace("%lists.proxies%", _runLists.combos.Count.ToString())

                .Replace("%run.ran%", _runInformations.ran.ToString())
                .Replace("%run.remaining%", (_runLists.combos.Count - _runInformations.ran).ToString())
                .Replace("%run.hits%", _runInformations.hits.ToString())
                .Replace("%run.free%", _runInformations.free.ToString())

                .Replace("%run.ran.percentage%", _runLists.combos.Count == 0 ? "0,00%" : ((double)_runInformations.ran / (double)_runLists.combos.Count).ToString("0.00%"))
                .Replace("%run.hits.percentage%", _runInformations.ran == 0 ? "0,00%" : ((double)_runInformations.hits / (double)_runInformations.ran).ToString("0.00%"))
                .Replace("%run.free.percentage%", _runInformations.hits == 0 ? "0,00%" : ((double)_runInformations.free / (double)_runInformations.hits).ToString("0.00%"))

                .Replace("%statistics.rpm%", _runStatistics.ranPerMinute.ToString())
                .Replace("%statistics.elapsed%", _runStatistics.GetElapsedTime())
                .Replace("%statistics.estimated%", _runStatistics.GetEstimatedTime());

            lock(_customStatistics.customStatisticsLocker)
                foreach (var customStatistic in _customStatistics.customStatistics)
                    text = text
                        .Replace($"%custom.{customStatistic.Key.Replace(" ", "_")}%", customStatistic.Value.ToString())
                        .Replace($"%custom.{customStatistic.Key.Replace(" ", "_")}.percentage%", customStatistic.Value == 0 ? "0,00%" : ((double)customStatistic.Value / (double)_runInformations.hits).ToString("0.00%"));

            return text;
        }

        public string CaptureDictionaryToString(CaptureDictionary captures)
        {
            _outputSettings = OutputSettings.GetOrNewInstance();

            string capture = null;

            foreach (var _capture in captures)
                capture += $"{FormatCapture(_capture)}{_outputSettings.capturesSeparator}";

            if (capture != null)
                capture = capture.Remove(capture.Length - _outputSettings.capturesSeparator.Length);

            return capture;
        }

        public string FormatCapture(KeyValuePair<string, string> pair)
        {
            _outputSettings = OutputSettings.GetOrNewInstance();

            return _outputSettings.captureFormat
                .Replace("%key%", pair.Key)
                .Replace("%name%", pair.Key)
                .Replace("%value%", pair.Value);
        }

        public string FormatOutput(string combo, string capture)
        {
            _outputSettings = OutputSettings.GetOrNewInstance();

            string text;

            if(capture == null)
                text = _outputSettings.outputFormat
                    .Replace("%combo%", combo);
            else
                text = _outputSettings.outputWithCaptureFormat
                    .Replace("%combo%", combo)
                    .Replace("%separator%", _outputSettings.comboCaptureSeparator)
                    .Replace("%capture%", capture);

            return text;
        }

        private static FormatUtils _classInstance;
        public static FormatUtils GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new FormatUtils());
        }
    }
}