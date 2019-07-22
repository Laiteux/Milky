using Milky.Run;
using Milky.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace Milky.Loops
{
    public class ConsoleLoops
    {
        private RunInformations _runInformations;
        private ConsoleUtils _consoleUtils;
        private DateTimeUtils _dateTimeUtils;

        public void StartTitleUpdateLoop()
        {
            _runInformations = RunInformations.GetOrNewInstance();
            _consoleUtils = ConsoleUtils.GetOrNewInstance();
            _dateTimeUtils = DateTimeUtils.GetOrNewInstance();

            Task.Run(() =>
            {
                while(_runInformations.runStatus != RunInformations.RunStatus.Finished)
                {
                    _consoleUtils.UpdateTitle();

                    Thread.Sleep(_dateTimeUtils.SecondsToMilliseconds(1));
                }
            });
        }

        private static ConsoleLoops _classInstance;
        public static ConsoleLoops GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new ConsoleLoops());
        }
    }
}