namespace Milky.Loops
{
    public class LoopsManager
    {
        private StatisticsLoops _statisticsLoop;
        private ConsoleLoops _consoleLoops;

        public void StartAllLoops()
        {
            _statisticsLoop = StatisticsLoops.GetOrNewInstance();
            _consoleLoops = ConsoleLoops.GetOrNewInstance();

            _statisticsLoop.StartRanPerMinuteLoop();
            _consoleLoops.StartTitleUpdateLoop();
        }

        private static LoopsManager _classInstance;
        public static LoopsManager GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new LoopsManager());
        }
    }
}