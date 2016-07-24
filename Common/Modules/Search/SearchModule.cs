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
                        string videoUrl = SearchWith.Youtube(e.Args[0]);
                        await message.Edit(videoUrl);
                    });
                cmd.CreateCommand("stackoverflow")
                    .Alias(new string[] { "so" })
                    .Description("Search StackOverflow for a question matching the provided text.")
                    .Parameter("tag", ParameterType.Required)
                    .Parameter("query", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var message = await e.Channel.SendMessage("Searching...");
                        string questionUrl = SearchWith.StackExchange("stackoverflow", e.Args[0], e.Args[1]);
                        await message.Edit(questionUrl);
                    });
                cmd.CreateCommand("superuser")
                    .Alias(new string[] { "su" })
                    .Description("Search SuperUser for a question matching the provided text.")
                    .Parameter("tag", ParameterType.Required)
                    .Parameter("query", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var message = await e.Channel.SendMessage("Searching...");
                        string questionUrl = SearchWith.StackExchange("superuser", e.Args[0], e.Args[1]);
                        await message.Edit(questionUrl);
                    });
                cmd.CreateCommand("arqade")
                    .Alias(new string[] { "aq" })
                    .Description("Search Arqade for a question matching the provided text.")
                    .Parameter("tag", ParameterType.Required)
                    .Parameter("query", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var message = await e.Channel.SendMessage("Searching...");
                        string questionUrl = SearchWith.StackExchange("arqade", e.Args[0], e.Args[1]);
                        await message.Edit(questionUrl);
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
                cmd.CreateCommand("lookup")
                    .Description("Retrieve information about the provided ip or domain.")
                    .Parameter("ip", ParameterType.Required)
                    .Do(async e =>
                    {
                        var message = await e.Channel.SendMessage("Searching...");
                        string getUrl = $"http://ip-api.com/json/{e.Args[0]}";
                        string mapsUrl = "https://www.google.com/maps/@{0},{1},15z";

                        using (WebClient client = new WebClient())
                        {
                            string json = client.DownloadString(getUrl);
                            var info = JObject.Parse(json);

                            if (info["status"].ToString() == "success")
                            {
                                var msg = new List<string>();
                                msg.Add($"**Map**: {string.Format(mapsUrl, info["lat"].ToString(), info["lon"].ToString())}");
                                msg.Add("```erlang");
                                msg.Add($" Country: {info["country"].ToString()}");
                                msg.Add($"  Region: {info["regionName"].ToString()}");
                                msg.Add($"    City: {info["city"].ToString()}");
                                msg.Add($"     Zip: {info["zip"].ToString()}");
                                msg.Add($"Timezone: {info["timezone"].ToString()}");
                                msg.Add($"     ISP: {info["isp"].ToString()}");
                                msg.Add("```");

                                await message.Edit(string.Join("\n", msg));
                            }
                            else
                            {
                                await message.Edit(info["message"].ToString());
                            }
                        }
                    });

                DogeyConsole.Log(LogSeverity.Info, "SearchModule", "Done");
            });
        } 
    }
}
