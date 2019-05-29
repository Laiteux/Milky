using Milky.Run;
using System;
using System.IO;
using System.Windows.Forms;

namespace Milky.Utils
{
    public class FileUtils
    {
        private RunLists _runLists;
        private ConsoleUtils _consoleUtils;

        private readonly object fileLocker = new object();

        public enum ComboType
        {
            Any,
            UsernamePassword,
            EmailPassword,
            GiftCode
        }

        public void LoadCombos(string comboType = null)
        {
            _runLists = RunLists.GetOrNewInstance();
            _consoleUtils = ConsoleUtils.GetOrNewInstance();

            string text = $"Select your{(comboType != null ? $" {comboType}" : null)} Combos File";

            _consoleUtils.Write($"{text} ");

            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = text,
                Filter = "Text File|*.txt"
            };

            while (dialog.ShowDialog() != DialogResult.OK)
            {
                string file = dialog.FileName;

                foreach (string line in File.ReadAllLines(file))
                    _runLists.combos.Add(line);

                if (_runLists.combos.Count != 0)
                    break;
            }

            Console.Clear();
        }

        public void LoadProxies(string proxyType = null)
        {
            _runLists = RunLists.GetOrNewInstance();
            _consoleUtils = ConsoleUtils.GetOrNewInstance();

            string text = $"Select your{(proxyType != null ? $" {proxyType}" : null)} Proxies File";

            _consoleUtils.Write($"{text} ");

            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = text,
                Filter = "Text File|*.txt"
            };

            while (dialog.ShowDialog() != DialogResult.OK)
            {
                string file = dialog.FileName;

                foreach (string line in File.ReadAllLines(file))
                    _runLists.proxies.Add(line);

                if (_runLists.proxies.Count != 0)
                    break;
            }

            Console.Clear();
        }

        public void CreateFile(string file, string directory)
        {
            lock (fileLocker)
            {
                Directory.CreateDirectory(directory);

                File.Create($"{directory}/{file}");
            }
        }

        public void WriteLine(string text, string file, string directory)
        {
            Directory.CreateDirectory(directory);

            lock(fileLocker)
                using (StreamWriter streamWriter = new StreamWriter($"{directory}/{file}", true))
                    streamWriter.WriteLine(text);
        }

        private static FileUtils _classInstance;
        public static FileUtils GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new FileUtils());
        }
    }
}