using Milky.Run;
using Milky.Statistics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Milky.Listeners
{
    public class KeyboardListener
    {
        private RunInformations _runInformations;
        private CustomStatistics _customStatistics;

        public void StartListening()
        {
            _runInformations = RunInformations.GetOrNewInstance();
            _customStatistics = CustomStatistics.GetOrNewInstance();

            Task.Run(() =>
            {
                while (_runInformations.runStatus == RunInformations.RunStatus.Running)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo pressed = Console.ReadKey(true);

                        switch (pressed.Key)
                        {
                            case ConsoleKey.S:
                                {
                                    _customStatistics.DisplayCustomStatistics();

                                    break;
                                }
                        }
                    }

                    Thread.Sleep(100);
                }
            });
        }

        private static KeyboardListener _classInstance;
        public static KeyboardListener GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new KeyboardListener());
        }
    }
}