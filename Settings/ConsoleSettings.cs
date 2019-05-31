namespace Milky.Settings
{
    public class ConsoleSettings
    {
        public string
            idleTitleFormat = "%program.name% %program.version% by %program.author%",
            runningTitleFormat =
                "%program.name% %program.version% by %program.author% – Running | " +
                "Ran : %run.ran% – Remaining : %run.remaining% – Hits : %run.hits% | " +
                "RPM : %statistics.rpm% – Elapsed : %statistics.elapsed% – Estimated : %statistics.estimated%",
            finishedTitleFormat =
                "%program.name% %program.version% by %program.author% – Finished | " +
                "Ran : %run.ran% – Hits : %run.hits% | " +
                "Elapsed : %statistics.elapsed%";

        public void SetTitleStyle(bool showFree, bool showPercentages)
        {
            runningTitleFormat =
                $"%program.name% %program.version% by %program.author% – Running | " +
                $"Ran : %run.ran%{(showPercentages ? " (%run.ran.percentage%)" : null)} – Remaining : %run.remaining% – Hits : %run.hits%{(showPercentages ? " (%run.hits.percentage%)" : null)}{(showFree ? $" – Free : %run.free%{(showPercentages ? " (%run.free.percentage%)" : null)}" : null)} | " +
                $"RPM : %statistics.rpm% – Elapsed : %statistics.elapsed% – Estimated : %statistics.estimated%";

            finishedTitleFormat =
                $"%program.name% %program.version% by %program.author% – Finished | " +
                $"Ran : %run.ran% – Hits : %run.hits%{(showPercentages ? " (%run.hits.percentage%)" : null)}{(showFree ? $" – Free : %run.free%{(showPercentages ? " (%run.free.percentage%)" : null)}" : null)} | " +
                $"Elapsed : %statistics.elapsed%";
        }

        private static ConsoleSettings _classInstance;
        public static ConsoleSettings GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new ConsoleSettings());
        }
    }
}