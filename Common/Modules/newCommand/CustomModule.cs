using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class CustomModule
    {
        private static ModuleManager _manager;
        private static DiscordClient _dogey;

        public static void Install(ModuleManager manager, ulong? sid = null, ulong? cid = null)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                IEnumerable<string> files = null;
                if (cid != null)
                {
                    string channel = $@"servers\{sid}\commands\{cid}";
                    cmd.AddCheck((cm, u, ch) => !ch.IsPrivate && ch.Id == cid);
                    files = Directory.EnumerateFiles(channel);
                } else
                if (sid != null)
                {
                    string server = $@"servers\{sid}\commands";
                    cmd.AddCheck((cm, u, ch) => !ch.IsPrivate && ch.Server.Id == sid);
                    files = Directory.EnumerateFiles(server);
                }
                
                if (files == null) return;
                foreach(string file in files)
                {
                    var command = new CustomCmd(file);

                    cmd.CreateCommand(command.Name)
                        .MinPermissions((int)AccessLevel.User)
                        .Description(command.Description)
                        .Parameter("index", ParameterType.Optional)
                        .Do(async e =>
                        {
                            string serverFolder = $@"servers\{e.Server.Id}\commands";
                            string channelFolder = $@"{serverFolder}\{e.Channel.Id}";

                            CustomCmd custom = null;
                            if (File.Exists(channelFolder))
                                custom = new CustomCmd(Path.Combine(channelFolder, $"{e.Command.Text}.doge"));
                            else if (File.Exists(serverFolder))
                                custom = new CustomCmd(Path.Combine(serverFolder, $"{e.Command.Text}.doge"));
                            else
                                return;

                            if (custom != null)
                            {
                                switch (custom.Messages.Count)
                                {
                                    case 0:
                                        await e.Channel.SendMessage($"This command has no stored messages, add some with `{custom.Name}.add [msg]`.");
                                        break;
                                    case 1:
                                        await e.Channel.SendMessage(CommandTags.Format(custom.Messages[0], e));
                                        break;
                                    default:
                                        int index;
                                        if (int.TryParse(e.Args[0], out index))
                                        {
                                            await e.Channel.SendMessage($"{index}. {CommandTags.Format(custom.Messages[index], e)}");
                                        } else
                                        {
                                            var r = new Random();
                                            index = r.Next(0, custom.Messages.Count);
                                            await e.Channel.SendMessage($"{index}. {CommandTags.Format(custom.Messages[index], e)}");
                                        }
                                        break;
                                }
                            }
                        });
                    cmd.CreateCommand($"{command.Name}.raw").Hide()
                        .MinPermissions((int)AccessLevel.ChannelMod)
                        .Description("View a message's raw text.")
                        .Parameter("index", ParameterType.Required)
                        .Do(async e =>
                        {
                            await Task.Delay(1);
                        });
                    cmd.CreateCommand($"{command.Name}.add").Hide()
                        .MinPermissions((int)AccessLevel.User)
                        .Description("Add a new message to the custom command.")
                        .Parameter("Message", ParameterType.Unparsed)
                        .Do(async e =>
                        {
                            await Task.Delay(1);
                        });
                    cmd.CreateCommand($"{command.Name}.del").Hide()
                        .MinPermissions((int)AccessLevel.ChannelMod)
                        .Description("Delete a message from a command at the specified index.")
                        .Parameter("index", ParameterType.Optional)
                        .Do(async e =>
                        {
                            await Task.Delay(1);
                        });
                    cmd.CreateCommand($"{command.Name}.info").Hide()
                        .MinPermissions((int)AccessLevel.User)
                        .Description("Get information about this command.")
                        .Do(async e =>
                        {
                            await Task.Delay(1);
                        });
                }
            });
        }
    }
}