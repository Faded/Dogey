using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using System.Collections.Generic;
using System.IO;

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
                        .Description(command.Description)
                        .Do(async e =>
                        {
                            await e.Channel.SendMessage(CommandTags.Format(command.Messages[0], e));
                        });
                    cmd.CreateCommand($"{command.Name}.raw").Hide()
                        .MinPermissions((int)AccessLevel.ChannelMod)
                        .Description("View a message's raw text.")
                        .Parameter("index", ParameterType.Required)
                        .Do(e => { return; });
                    cmd.CreateCommand($"{command.Name}.add").Hide()
                        .MinPermissions((int)AccessLevel.User)
                        .Description("Add a new message to the custom command.")
                        .Parameter("Message", ParameterType.Unparsed)
                        .Do(e => { return; });
                    cmd.CreateCommand($"{command.Name}.del").Hide()
                        .MinPermissions((int)AccessLevel.ChannelMod)
                        .Description("Delete a message from a command at the specified index.")
                        .Parameter("index", ParameterType.Optional)
                        .Do(e => { return; });
                    cmd.CreateCommand($"{command.Name}.info").Hide()
                        .MinPermissions((int)AccessLevel.User)
                        .Description("Get information about this command.")
                        .Do(e => { return; });
                }
            });
        }
    }
}