using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using Dogey.Utility;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Dogey.Common.Modules
{
    public class GuildModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("activity", cmd =>
            {
                cmd.MinPermissions((int)AccessLevel.ServerAdmin);

                cmd.CreateCommand("channel")
                    .Description("Get or set the activity channel.")
                    .Parameter("name/id/mention", ParameterType.Required)
                    .Do(async e =>
                    {
                        string serverConfig = $@"servers\{e.Server.Id}\guild.json";
                        string channel = e.Args[0];

                        if (!File.Exists(serverConfig))
                            File.WriteAllText(serverConfig, JsonConvert.SerializeObject(new GuildSettings(e.Server.Id)));

                        var config = JsonConvert.DeserializeObject<GuildSettings>(File.ReadAllText(serverConfig));
                        if (string.IsNullOrWhiteSpace(channel))
                        {
                            if (config.ActivityChannel == null)
                            {
                                await e.Channel.SendMessage("There is not a configured activity channel for this server.");
                            } else
                            {
                                await e.Channel.SendMessage($"The current activity channel is {e.Server.GetChannel(config.ActivityChannel ?? 0).Mention}");
                            }
                        } else
                        {
                            Channel c = null;
                            if (e.Message.MentionedUsers.Count() == 1)
                            {
                                c = e.Message.MentionedChannels.FirstOrDefault();
                            } else
                            if (e.Server.FindChannels(channel).Any())
                            {
                                c = e.Server.FindChannels(channel).FirstOrDefault();
                            } else
                            if (e.Server.AllChannels.Any(x => x.Id.ToString() == channel))
                            {
                                c = e.Server.GetChannel(ulong.Parse(channel));
                            } else
                            {
                                await e.Channel.SendMessage($"I was unable to find a channel like `{channel}`.");
                                return;
                            }

                            config.ActivityChannel = c.Id;
                            config.EnableActivity = true;

                            File.WriteAllText(serverConfig, JsonConvert.SerializeObject(config));

                            await e.Channel.SendMessage($"The channel {c.Mention} will now be used to log server activity.");
                        }
                    });
                cmd.CreateCommand("toggle")
                    .Description("Toggle logging guild activity to the configured channel.")
                    .Do(async e =>
                    {
                        string serverConfig = $@"servers\{e.Server.Id}\guild.json";

                        if (!File.Exists(serverConfig))
                            File.WriteAllText(serverConfig, JsonConvert.SerializeObject(new GuildSettings(e.Server.Id)));

                        var config = JsonConvert.DeserializeObject<GuildSettings>(File.ReadAllText(serverConfig));
                        if (config.ActivityChannel == null)
                        {
                            await e.Channel.SendMessage("You need to configure an activity channel before you can toggle logging.");
                        } else
                        {
                            config.EnableActivity = (config.EnableActivity) ? false : true;
                            string mode = (config.EnableActivity) ? "enabled" : "disabled";

                            File.WriteAllText(serverConfig, JsonConvert.SerializeObject(config));

                            await e.Channel.SendMessage($"Logging has been {mode} for the channel {e.Server.GetChannel(ulong.Parse(config.ActivityChannel.ToString())).Mention}.");
                        }
                    });
            });

            manager.CreateCommands("stars", cmd =>
            {
                cmd.CreateCommand("channel")
                    .Description("Set the star channel.")
                    .Parameter("id/mention", ParameterType.Required)
                    .Do(async e =>
                    {
                        await e.User.SendMessage("Don't");
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "GuildModule", "Done");
        }
    }
}