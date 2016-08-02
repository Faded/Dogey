using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Dogey.Common.Modules
{
    public class TempModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("tempvoice")
                    .Description("Create a temporary voice channel.")
                    .Parameter("name", ParameterType.Optional)
                    .Do(async e =>
                    {
                        if (e.User.VoiceChannel == null)
                        {
                            await e.Channel.SendMessage("You must be in a voice channel before you can create a temporary one.");
                            return;
                        }

                        string serverFolder = $@"servers\{e.Server.Id}";

                        string name;
                        do
                        {
                            name = ChannelTitle.GetTitle();
                        } while (e.Server.VoiceChannels.Any(x => x.Name.Contains(name)));

                        var channel = await e.Server.CreateChannel(name, ChannelType.Voice);

                        var tChannel = new TempChannel()
                        {
                            Id = channel.Id,
                            Name = name,
                            CreatedBy = e.User.Id,
                            CreatedOn = DateTime.Now
                        };

                        File.WriteAllText(Path.Combine(serverFolder, $"{channel.Id}.voice"), JsonConvert.SerializeObject(tChannel));

                        if (e.User != e.Server.Owner)
                            await e.User.Edit(voiceChannel: channel);
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "TempModule", "Done");
        }
    }
}