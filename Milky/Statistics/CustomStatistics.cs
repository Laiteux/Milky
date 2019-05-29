using System.Collections.Generic;

namespace Milky.Statistics
{
    public class CustomStatistics
    {
        public readonly Dictionary<string, int> customStatistics = new Dictionary<string, int>();

        public readonly object customStatisticsLocker = new object();

        public void AddCustomStatistic(string name, int value = 0)
        {
            lock(customStatisticsLocker)
                customStatistics.Add(name, value);
        }

        public void UpdateCustomStatistic(string name, int value)
        {
            lock(customStatisticsLocker)
                customStatistics[name] = value;
        }

        public void IncrementCustomStatistic(string name, int increment = 1)
        {
            lock (customStatisticsLocker)
                customStatistics[name] += increment;
        }

        private static CustomStatistics _classInstance;
        public static CustomStatistics GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new CustomStatistics());
        }
    }
}