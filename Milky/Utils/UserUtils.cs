using System;

namespace Milky.Utils
{
    public class UserUtils
    {
        private ConsoleUtils _consoleUtils;

        public string AskString(string asked)
        {
            return AskUser(asked);
        }

        public int AskInteger(string asked)
        {
            while (true)
            {
                try
                {
                    return int.Parse(AskUser(asked));
                }
                catch { }
            }
        }

        private string AskUser(string asked)
        {
            _consoleUtils = ConsoleUtils.GetOrNewInstance();

            _consoleUtils.Write($"{asked} : ");
            string answer = Console.ReadLine();
            Console.Clear();

            return answer;
        }

        public string AskChoice(string asked, string[] choices)
        {
            _consoleUtils = ConsoleUtils.GetOrNewInstance();

			Console.Clear();

            while (true)
            {
                try
                {
                    _consoleUtils.WriteLine($"{asked}{Environment.NewLine}");

                    for(int i = 0; i < choices.Length; i++)
                        _consoleUtils.WriteLine($"[{i + 1}] {choices[i]}");

                    _consoleUtils.Write($"{Environment.NewLine}Select : ");
                    string answer = Console.ReadLine();
                    Console.Clear();

                    return choices[int.Parse(answer) - 1];
                }
                catch { }
            }
        }

        private static UserUtils _classInstance;
        public static UserUtils GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new UserUtils());
        }
    }
}