using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using Dogey.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dogey.Common.Modules
{
    public class CommandModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("commands", cmd =>
            {
                cmd.AddCheck((cm, u, ch) => !ch.IsPrivate);

                cmd.CreateCommand("")
                    .MinPermissions((int)AccessLevel.User)
                    .Alias(new string[] { "list" })
                    .Description("List of available custom commands.")
                    .Do(async e =>
                    {
                        string serverFolder = $@"servers\{e.Server.Id}\commands";
                        string channelFolder = $@"{serverFolder}\{e.Channel.Id}";

                        var msg = new List<string>();
                        msg.Add("```erlang");

                        var serverCmds = Directory.EnumerateFiles(serverFolder).Select(x => Path.GetFileNameWithoutExtension(x));
                        msg.Add($"{e.Server.Name}: {string.Join(", ", serverCmds)}");

                        if (Directory.Exists(channelFolder))
                        {
                            var channelCmds = Directory.EnumerateFiles(channelFolder).Select(x => Path.GetFileNameWithoutExtension(x));
                            msg.Add($"{e.Channel.Name}: {string.Join(", ", channelCmds)}");
                        }

                        msg.Add("```");

                        await e.Channel.SendMessage(string.Join("\n", msg));
                    });
                cmd.CreateCommand("create")
                    .MinPermissions((int)AccessLevel.User)
                    .Alias(new string[] { "new" })
                    .Description("Create new custom command.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("");
                    });
                cmd.CreateCommand("delete")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Alias(new string[] { "remove" })
                    .Description("Delete existing custom command.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("");
                    });
                cmd.CreateCommand("find")
                    .MinPermissions((int)AccessLevel.User)
                    .Alias(new string[] { "search" })
                    .Description("Find exsting custom command.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("");
                    });
                cmd.CreateCommand("deleted")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Alias(new string[] { "removed" })
                    .Description("Find deleted custom command.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("");
                    });
                cmd.CreateCommand("restore")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Restore deleted custom command.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("");
                    });
                cmd.CreateCommand("toggle")
                    .MinPermissions((int)AccessLevel.ServerMod)
                    .Description("Toggle whether the current channel has its own set of custom commands.")
                    .Do(async e =>
                    {
                        string serverFolder = $@"servers\{e.Server.Id}\commands";
                        string channelFolder = $@"{serverFolder}\{e.Channel.Id}";

                        if (Directory.Exists(channelFolder))
                        {
                            Directory.Move(channelFolder, channelFolder + "_disabled");
                            await e.Channel.SendMessage($"Unique commands have been **disabled** for {e.Channel.Mention}.");
                        } else
                        if (Directory.Exists(channelFolder + "_disabled"))
                        {
                            Directory.Move(channelFolder + "_disabled", channelFolder);
                            await e.Channel.SendMessage($"Unique commands have been **enabled** for {e.Channel.Mention}.");
                        } else
                        {
                            Directory.CreateDirectory(channelFolder);
                            await e.Channel.SendMessage($"Unique commands have been **enabled** for {e.Channel.Mention}.");
                        }
                    });
            });
            
            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("*.raw")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("View a message's raw text.")
                    .Parameter("index", ParameterType.Required)
                    .Do(e => { return; });
                cmd.CreateCommand("*.add")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Add a new message to the custom command.")
                    .Parameter("Message", ParameterType.Unparsed)
                    .Do(e => { return; });
                cmd.CreateCommand("*.del")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Delete a message from a command at the specified index.")
                    .Parameter("index", ParameterType.Optional)
                    .Do(e => { return; });
                cmd.CreateCommand("*.info")
                    .MinPermissions((int)AccessLevel.User)
                    .Description("Get information about this command.")
                    .Do(e => { return; });
            });

            LoadExistingCommands();
            DogeyConsole.Log(LogSeverity.Info, "CommandModule", "Done");
        }

        private void LoadExistingCommands()
        {
            int servers = 0, channels = 0, commands = 0;
            if (!Directory.Exists("servers")) Directory.CreateDirectory("servers");

            foreach (string serverFolder in Directory.EnumerateDirectories("servers"))
            {
                ulong sid = ulong.Parse(new DirectoryInfo(serverFolder).Name);
                foreach (string channelFolder in Directory.EnumerateDirectories(Path.Combine(serverFolder, "commands")))
                {
                    ulong cid;
                    if (ulong.TryParse(new DirectoryInfo(channelFolder).Name, out cid))
                    {
                        CustomModule.Install(_manager, sid, cid);

                        commands += Directory.EnumerateFiles(channelFolder).Count();
                        channels++;
                    }
                }

                CustomModule.Install(_manager, sid);
                commands += Directory.EnumerateFiles(Path.Combine(serverFolder, "commands")).Count();
            }

            CustomModule.Install(_manager);
            servers = Directory.EnumerateDirectories("servers").Count();

            DogeyConsole.Log(LogSeverity.Info, "CommandModule", $"Loaded {commands} command(s) for {servers} server(s) and {channels} channel(s).");
        }
    }
}