using Milky.Run;
using System;
using System.Threading;
using Milky.Settings;

namespace Milky.Utils
{
    public class ConsoleUtils
    {
        private RunInformations _runInformations;
        private ConsoleSettings _consoleSettings;
        private FormatUtils _formatUtils;

        private readonly object consoleLocker = new object();

        public void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            lock (consoleLocker)
            {
                Console.ForegroundColor = color;
                Console.Write(text);
            }
        }

        public void WriteLine(string text, ConsoleColor color = ConsoleColor.White)
        {
            lock (consoleLocker)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(text);
            }
        }

        public void Exit(string message, int delay = 3000, ConsoleColor color = ConsoleColor.White)
        {
            lock (consoleLocker)
            {
                Console.Clear();
                Console.ForegroundColor = color;
                Write($"{message}, exiting ... ");

                Thread.Sleep(delay);
                Environment.Exit(0);
            }
        }

        public void UpdateTitle()
        {
            _runInformations = RunInformations.GetOrNewInstance();
            _consoleSettings = ConsoleSettings.GetOrNewInstance();
            _formatUtils = FormatUtils.GetOrNewInstance();

            string title = null;

            switch (_runInformations.runStatus)
            {
                case RunInformations.RunStatus.Idle:
                    {
                        title = _formatUtils.FormatTitle(_consoleSettings.idleTitleFormat);

                        break;
                    }
                case RunInformations.RunStatus.Running:
                    {
                        title = _formatUtils.FormatTitle(_consoleSettings.runningTitleFormat);

                        break;
                    }
                case RunInformations.RunStatus.Finished:
                    {
                        title = _formatUtils.FormatTitle(_consoleSettings.finishedTitleFormat);

                        break;
                    }
            }

            Console.Title = title;
        }

        private static ConsoleUtils _classInstance;
        public static ConsoleUtils GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new ConsoleUtils());
        }
    }
}