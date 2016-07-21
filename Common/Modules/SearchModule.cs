using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class SearchModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("youtube")
                    .Alias(new string[] { "yt" })
                    .Description("Search youtube for a video matching the specified text.")
                    .Parameter("keywords", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var message = await e.Channel.SendMessage("Searching...");
                        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                        {
                            ApiKey = Program.config.Token.Google,
                            ApplicationName = this.GetType().ToString()
                        });

                        var searchListRequest = youtubeService.Search.List("snippet");
                        searchListRequest.Q = e.Args[0];
                        searchListRequest.MaxResults = 50;

                        var searchListResponse = await searchListRequest.ExecuteAsync();
                        
                        foreach (var searchResult in searchListResponse.Items)
                        {
                            if (searchResult.Id.Kind == "youtube#video")
                            {
                                await message.Edit($"https://www.youtube.com/watch?v={searchResult.Id.VideoId}");
                                return;
                            }
                        }
                    });
                cmd.CreateCommand("gif")
                    .Description("Search for a gif matching the specified text.")
                    .Parameter("keywords", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var message = await e.Channel.SendMessage("Searching...");
                        string getUrl = $"http://api.giphy.com/v1/gifs/random?api_key=dc6zaTOxFJmzC&tag={Uri.UnescapeDataString(e.Args[0])}";

                        string image;
                        using (WebClient client = new WebClient())
                        {
                            string json = client.DownloadString(getUrl);
                            image = JObject.Parse(json)["data"]["image_original_url"].ToString();
                        }

                        await message.Edit(image);
                    });
                cmd.CreateCommand("users")
                    .Description("Search for a user that matches the provided text.")
                    .Parameter("keywords", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var message = await e.Channel.SendMessage("Searching...");
                        string keywords = e.Args[0];
                        var nicknames = e.Server.Users.Where(x => x.Nickname.Contains(keywords));
                        var usernames = e.Server.Users.Where(x => x.Name.Contains(keywords));
                        var discrims = e.Server.Users.Where(x => x.Discriminator.ToString().Contains(keywords));

                        var msg = new List<string>();
                        msg.Add("```erlang");

                        if (nicknames.Count() > 0)
                        {
                            msg.Add($"Nicknames({nicknames.Count()}) " + string.Join(", ", nicknames));
                        }
                        if (usernames.Count() > 0)
                        {
                            msg.Add($"Usernames({usernames.Count()}) " + string.Join(", ", usernames));
                        }
                        if (discrims.Count() > 0)
                        {
                            msg.Add($"Discriminators({discrims.Count()}) " + string.Join(", ", discrims));
                        }

                        msg.Add("```");

                        if (msg.Count <= 2)
                        {
                            await message.Edit($"There are not any users like the keyword `{keywords}`.");
                        } else
                        {
                            await message.Edit(string.Join("\n", msg));
                        }

                    });

                DogeyConsole.Log(LogSeverity.Info, "SearchModule", "Loaded.");
            });
        } 
    }
}
