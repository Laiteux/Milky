using Milky.Utils;
using System.Collections.Generic;

namespace Milky.Statistics
{
    public class CustomStatistics
    {
        private ConsoleUtils _consoleUtils;

        public readonly Dictionary<string, int> customStatistics = new Dictionary<string, int>();
        public readonly object customStatisticsLocker = new object();

        public void AddCustomStatistic(string name, int value = 0)
        {
            lock (customStatisticsLocker)
                customStatistics.Add(name, value);
        }

        public void UpdateCustomStatistic(string name, int value)
        {
            lock (customStatisticsLocker)
                customStatistics[name] = value;
        }

        public void IncrementCustomStatistic(string name, int increment = 1)
        {
            lock (customStatisticsLocker)
                customStatistics[name] += increment;
        }

        public void DisplayCustomStatistics()
        {
            _consoleUtils = ConsoleUtils.GetOrNewInstance();

            lock (customStatisticsLocker)
                foreach (var customStatistic in customStatistics)
                    _consoleUtils.WriteLine($"{customStatistic.Key} = {customStatistic.Value}");
        }

        private static CustomStatistics _classInstance;
        public static CustomStatistics GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new CustomStatistics());
        }
    }
}