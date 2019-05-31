using Milky.Run;
using Milky.Statistics;
using Milky.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace Milky.Loops
{
    public class StatisticsLoops
    {
        private RunInformations _runInformations;
        private RunStatistics _runStatistics;
        private DateTimeUtils _dateTimeUtils;

        public void StartRanPerMinuteLoop()
        {
            _runInformations = RunInformations.GetOrNewInstance();
            _runStatistics = RunStatistics.GetOrNewInstance();
            _dateTimeUtils = DateTimeUtils.GetOrNewInstance();

            Task.Run(() =>
            {
                while (_runInformations.runStatus != RunInformations.RunStatus.Finished)
                {
                    int ranBefore = _runInformations.ran;
                    Thread.Sleep(_dateTimeUtils.SecondsToMilliseconds(3));
                    int ranAfter = _runInformations.ran;

                    _runStatistics.ranPerMinute = (ranAfter - ranBefore) * 20;
                }
            });
        }

        private static StatisticsLoops _classInstance;
        public static StatisticsLoops GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new StatisticsLoops());
        }
    }
}