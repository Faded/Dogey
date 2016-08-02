using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System.Net;
using System.Text.RegularExpressions;

namespace Dogey.Common.Modules
{
    public class GamesModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("overwatch")
                    .Alias(new string[] { "ow" })
                    .Description("Get overwatch stats for this user.")
                    .Parameter("user", ParameterType.Required)
                    .Do(async e =>
                    {
                        var message = await e.Channel.SendMessage("Searching...");
                        const string baseUrl = "http://masteroverwatch.com/profile/pc/us/";
                        string battletag = e.Args[0];
                        
                        string userUrl = baseUrl + battletag.Replace("#", "-");

                        using (var c = new WebClient())
                        {
                            string title = Regex.Match(c.DownloadString(userUrl),
                                @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
                                RegexOptions.IgnoreCase).Groups["Title"].Value;

                            if (title.Contains("Not found."))
                            {
                                await message.Edit($"The battletag `{battletag}` is not valid.");
                                return;
                            }
                        }
                        
                        await message.Edit(userUrl);
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "GamesModule", "Done");
        }
    }
}