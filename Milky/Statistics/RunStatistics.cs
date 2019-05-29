using Milky.Run;
using Milky.Utils;
using System;

namespace Milky.Statistics
{
    public class RunStatistics
    {
        private DateTimeUtils _dateTimeUtils;
        private RunLists _runLists;
        private RunInformations _runInformations;

        public int ranPerMinute;

        public string GetElapsedTime()
        {
            _dateTimeUtils = DateTimeUtils.GetOrNewInstance();
            _runInformations = RunInformations.GetOrNewInstance();

            return TimeSpan.FromSeconds(_dateTimeUtils.GetCurrentUnixTimeSeconds() - _runInformations.runStartUnixTimeSeconds).ToString();
        }

        public string GetEstimatedTime()
        {
            _runLists = RunLists.GetOrNewInstance();
            _runInformations = RunInformations.GetOrNewInstance();

            int estimatedSeconds;

            try
            {
                estimatedSeconds = (_runLists.combos.Count - _runInformations.ran) / (ranPerMinute / 60);
            }
            catch
            {
                estimatedSeconds = 0;
            }

            return TimeSpan.FromSeconds(estimatedSeconds).ToString();
        }

        private static RunStatistics _classInstance;
        public static RunStatistics GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new RunStatistics());
        }
    }
}