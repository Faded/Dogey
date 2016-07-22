using Discord;
using Discord.Commands;
using Discord.Commands.Permissions;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                cmd.CreateCommand("")
                    .MinPermissions((int)AccessLevel.User)
                    .Description("Displays a list of all available custom commands for this server.")
                    .Do(async e =>
                    {
                        string serverFolder = $@"servers\{e.Server.Id}\commands";
                        string channelFolder = Path.Combine(serverFolder, e.Channel.Id.ToString());
                        
                        if (!Directory.Exists(channelFolder))
                            Directory.CreateDirectory(channelFolder);
                        
                        var serverCommands = new List<string>();
                        var sfiles = new DirectoryInfo(serverFolder).GetFiles("*.doge");
                        foreach (FileInfo file in sfiles)
                        {
                            serverCommands.Add(file.Name.Replace(".doge", ""));
                        }

                        var channelCommands = new List<string>();
                        var cfiles = new DirectoryInfo(channelFolder).GetFiles("*.doge");
                        foreach (FileInfo file in cfiles)
                        {
                            channelCommands.Add(file.Name.Replace(".doge", ""));
                        }

                        var finalMsg = new List<string>();
                        if (serverCommands.Count > 0)
                        {
                            finalMsg.Add($"{e.Server.Name} ({serverCommands.Count()}): {string.Join(", ", serverCommands)}");
                        }
                        if (channelCommands.Count > 0)
                        {
                            string channelTitleCase = new CultureInfo("en-US", false).TextInfo.ToTitleCase(e.Channel.Name);
                            finalMsg.Add($"{channelTitleCase} ({channelCommands.Count()}): {string.Join(", ", channelCommands)}");
                        }

                        if (finalMsg.Count > 0)
                        {
                            await e.Channel.SendMessage($"```erlang\n{string.Join("\n", finalMsg)}```");
                        } else
                        {
                            await e.Channel.SendMessage("This server does not have any available commands.");
                        }
                    });
                cmd.CreateCommand("deleted")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Displays a list of all recently deleted commands for this server.")
                    .Do(async e =>
                    {
                        string serverFolder = $@"servers\{e.Server.Id}\commands";
                        string channelFolder = Path.Combine(serverFolder, e.Channel.Id.ToString());

                        if (!Directory.Exists(channelFolder))
                            Directory.CreateDirectory(channelFolder);
                        
                        var serverCommands = new List<string>();
                        var sfiles = new DirectoryInfo(serverFolder).GetFiles("*.del");
                        foreach (FileInfo file in sfiles)
                        {
                            serverCommands.Add(file.Name.Replace(".del", ""));
                        }

                        var channelCommands = new List<string>();
                        var cfiles = new DirectoryInfo(channelFolder).GetFiles("*.del");
                        foreach (FileInfo file in cfiles)
                        {
                            channelCommands.Add(file.Name.Replace(".del", ""));
                        }

                        var finalMsg = new List<string>();
                        if (serverCommands.Count > 0)
                        {
                            finalMsg.Add($"{e.Server.Name} ({serverCommands.Count()}): {string.Join(", ", serverCommands)}");
                        }
                        if (channelCommands.Count > 0)
                        {
                            string channelTitleCase = new CultureInfo("en-US", false).TextInfo.ToTitleCase(e.Channel.Name);
                            finalMsg.Add($"{channelTitleCase} ({channelCommands.Count()}): {string.Join(", ", channelCommands)}");
                        }

                        if (finalMsg.Count > 0)
                        {
                            await e.Channel.SendMessage($"```erlang\n{string.Join("\n", finalMsg)}```");
                        }
                        else
                        {
                            await e.Channel.SendMessage("This server does not have any recently deleted commands.");
                        }
                    });
                cmd.CreateCommand("find")
                    .MinPermissions((int)AccessLevel.User)
                    .Description("Displays a list of all available custom commands for this server.")
                    .Alias(new string[] { "search" })
                    .Parameter("text", ParameterType.Required)
                    .Do(async e =>
                    {
                        string serverFolder = $@"servers\{e.Server.Id}\commands";
                        string channelFolder = Path.Combine(serverFolder, e.Channel.Id.ToString());

                        if (!Directory.Exists(channelFolder))
                            Directory.CreateDirectory(channelFolder);
                        
                        var serverCommands = new List<string>();
                        var sfiles = new DirectoryInfo(serverFolder).GetFiles("*.doge");
                        foreach (FileInfo file in sfiles)
                        {
                            serverCommands.Add(file.Name.Replace(".doge", ""));
                        }
                        serverCommands = serverCommands.Where(x => x.Contains(e.Args[0])).ToList();

                        var channelCommands = new List<string>();
                        var cfiles = new DirectoryInfo(channelFolder).GetFiles("*.doge");
                        foreach (FileInfo file in cfiles)
                        {
                            channelCommands.Add(file.Name.Replace(".doge", ""));
                        }
                        channelCommands = channelCommands.Where(x => x.Contains(e.Args[0])).ToList();

                        var finalMsg = new List<string>();
                        if (serverCommands.Count > 0)
                        {
                            finalMsg.Add($"{e.Server.Name} ({serverCommands.Count()}): {string.Join(", ", serverCommands)}");
                        }
                        if (channelCommands.Count > 0)
                        {
                            string channelTitleCase = new CultureInfo("en-US", false).TextInfo.ToTitleCase(e.Channel.Name);
                            finalMsg.Add($"{channelTitleCase} ({channelCommands.Count()}): {string.Join(", ", channelCommands)}");
                        }

                        if (finalMsg.Count > 0)
                        {
                            await e.Channel.SendMessage($"```erlang\n{string.Join("\n", finalMsg)}```");
                        }
                        else
                        {
                            await e.Channel.SendMessage($"I could not find any commands like `{e.Args[0]}`.");
                        }
                    });
                cmd.CreateCommand("create")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Create a new custom command.")
                    .Alias(new string[] { "new" })
                    .Parameter("name", ParameterType.Required)
                    .Parameter("message", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        string serverFolder = $@"servers\{e.Server.Id}\commands";
                        string channelFolder = Path.Combine(serverFolder, e.Channel.Id.ToString());

                        if (string.IsNullOrWhiteSpace(e.Args[1]))
                        {
                            await e.Channel.SendMessage("The parameter `message` requires an input.");
                            return;
                        }

                        if (!Directory.Exists(serverFolder)) Directory.CreateDirectory(serverFolder);

                        string commandText = Regex.Replace(e.Args[0], @"[^\w\s]", "");

                        if (File.Exists(Path.Combine(serverFolder, commandText + ".doge")))
                        {
                            await e.Channel.SendMessage($"The command `{commandText}` already exists.");
                            return;
                        }

                        var command = new CommandInfo()
                        {
                            Name = commandText,
                            CreatedBy = e.User.Id,
                            CreatedOn = DateTime.Now,
                            EditedBy = e.User.Id,
                            EditedOn = DateTime.Now,
                            Bind = Bind.Server,
                            BoundTo = e.Server.Id
                        };
                        command.Messages.Add(e.Args[1]);

                        string json = JsonConvert.SerializeObject(command, Formatting.Indented);
                        File.Create(Path.Combine(serverFolder, command.Name + ".doge")).Close();
                        File.WriteAllText(Path.Combine(serverFolder, command.Name + ".doge"), json);

                        CreateCommand(manager, command);

                        await e.Channel.SendMessage($"The command `{command.Name}` has been created.");
                    });
                cmd.CreateCommand("delete")
                    .MinPermissions((int)AccessLevel.ChannelAdmin)
                    .Description("Delete an existing custom command.")
                    .Parameter("name", ParameterType.Required)
                    .Do(async e =>
                    {
                        string serverCommand = $@"servers\{e.Server.Id}\commands\{e.Args[0]}.doge";
                        string channelCommand = $@"servers\{e.Server.Id}\commands\{e.Channel.Id}\{e.Args[0]}.doge";

                        if (File.Exists(serverCommand))
                        {
                            File.Move(serverCommand, serverCommand.Replace($".doge", ".del"));
                        } else
                        if (File.Exists(channelCommand))
                        {
                            File.Move(channelCommand, channelCommand.Replace($".doge", ".del"));
                        } else
                        {
                            await e.Channel.SendMessage($"{e.Args[0]} is not an existing command.");
                        }
                        
                        await e.Channel.SendMessage($"The command `{e.Args[0]}` has been deleted.");
                    });
                cmd.CreateCommand("restore")
                    .MinPermissions((int)AccessLevel.ChannelAdmin)
                    .Description("Restore a recently deleted command.")
                    .Parameter("text", ParameterType.Required)
                    .Do(async e =>
                    {
                        string serverFolder = $@"servers\{e.Server.Id}\commands";
                        string channelFolder = Path.Combine(serverFolder, e.Channel.Id.ToString());

                        var commands = new List<string>();
                        var dir = new DirectoryInfo(serverFolder);
                        var commandFiles = dir.GetFiles("*.del");
                        var deletedFile = commandFiles.Where(x => x.Name.Contains(e.Args[0])).FirstOrDefault();

                        if (deletedFile != null)
                        {
                            File.Move(deletedFile.FullName, deletedFile.FullName.Replace(".del", ".doge"));
                            CreateCommand(manager, JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(deletedFile.FullName)));
                            await e.Channel.SendMessage($"The command `{e.Args[0]}` has been restored.");
                        }
                        else
                        {
                            await e.Channel.SendMessage($"I could not find any command like `{e.Args[0]}`.");
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

            LoadExistingCommands(manager);

            DogeyConsole.Log(LogSeverity.Info, "CommandModule", "Done");
        }

        public void LoadExistingCommands(ModuleManager manager)
        {
            int servers = 0;
            int channels = 0;
            int commands = 0;
            if (!Directory.Exists("servers")) Directory.CreateDirectory("servers");
            foreach (string folder in Directory.GetDirectories("servers"))
            {
                string commandFolder = Path.Combine(folder, "commands");
                if (!Directory.Exists(commandFolder)) Directory.CreateDirectory(commandFolder);

                foreach (string channel in Directory.GetDirectories(commandFolder))
                {
                    foreach (string file in Directory.GetFiles(channel))
                    {
                        var cmd = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(file));

                        CreateCommand(manager, cmd);
                        commands++;
                    }
                    channels++;
                }
                foreach (string file in Directory.GetFiles(commandFolder))
                {
                    var cmd = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(file));

                    CreateCommand(manager, cmd);
                    commands++;
                }
                servers++;
            }

            DogeyConsole.Log(LogSeverity.Info, "CommandModule", $"Loaded {commands} command(s) for {servers} server(s) and {channels} channel(s).");
        }

        public void CreateCommand(ModuleManager manager, CommandInfo command)
        {
            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand(command.Name)
                    .MinPermissions((int)AccessLevel.User)
                    .Description("Create a new custom command.")
                    .Parameter("index", ParameterType.Optional)
                    .Do(async e =>
                    {
                        string serverCommand = $@"servers\{e.Server.Id}\commands\{e.Command.Text}.doge";
                        string channelCommand = $@"servers\{e.Server.Id}\commands\{e.Channel.Id}\{e.Command.Text}.doge";

                        CommandInfo cmdObj = null;
                        if (File.Exists(serverCommand))
                        {
                            cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(serverCommand));
                        } else
                        if (File.Exists(channelCommand))
                        {
                            cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(channelCommand));
                        } else
                        {
                            return;
                        }
                        
                        switch (cmdObj.Messages.Count)
                        {
                            case 0:
                                await e.Channel.SendMessage($"This command has no stored messages, add some with `{cmdObj.Name}.add [msg]`.");
                                return;
                            case 1:
                                await e.Channel.SendMessage(CommandTags.Format(cmdObj.Messages[0], e));
                                return;
                            default:
                                if (!string.IsNullOrWhiteSpace(e.Args[0]))
                                {
                                    int i;
                                    if (Int32.TryParse(e.Args[0], out i))
                                        await e.Channel.SendMessage($"{i}. {CommandTags.Format(cmdObj.Messages[i], e)}");
                                } else
                                {
                                    string msg = cmdObj.Messages[new Random().Next(0, cmdObj.Messages.Count())];
                                    int index = cmdObj.Messages.IndexOf(msg);
                                    await e.Channel.SendMessage($"{index}. {msg}");
                                }
                                return;
                        }
                    });
                cmd.CreateCommand($"{command.Name}.raw").Hide()
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("View a message's raw text.")
                    .Parameter("index", ParameterType.Required)
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string serverCommand = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        string channelCommand = $@"servers\{e.Server.Id}\commands\{e.Channel.Id}\{commandName}.doge";

                        CommandInfo cmdObj = null;
                        if (File.Exists(serverCommand))
                        {
                            cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(serverCommand));
                        }
                        else
                        if (File.Exists(channelCommand))
                        {
                            cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(channelCommand));
                        }
                        else
                        {
                            return;
                        }

                        switch (cmdObj.Messages.Count)
                        {
                            case 0:
                                await e.Channel.SendMessage($"This command has no stored messages, add some with `{cmdObj.Name}.add [msg]`.");
                                return;
                            case 1:
                                await e.Channel.SendMessage(cmdObj.Messages[0]);
                                return;
                            default:
                                if (!string.IsNullOrWhiteSpace(e.Args[0]))
                                {
                                    int i;
                                    if (Int32.TryParse(e.Args[0], out i))
                                        await e.Channel.SendMessage($"{i}. {CommandTags.Format(cmdObj.Messages[i], e)}");
                                }
                                return;
                        }
                    });
                cmd.CreateCommand($"{command.Name}.add").Hide()
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Add a new message to the custom command.")
                    .Parameter("Message", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string commandFile = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        if (!File.Exists(commandFile)) return;
                        if (string.IsNullOrEmpty(e.Args[0])) return;

                        var cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(commandFile));
                        cmdObj.Messages.Add(e.Args[0]);
                        cmdObj.EditedBy = e.User.Id;
                        cmdObj.EditedOn = DateTime.Now;

                        File.WriteAllText(commandFile, JsonConvert.SerializeObject(cmdObj));

                        await e.Channel.SendMessage($"Added message #{cmdObj.Messages.Count()} to `{cmdObj.Name}`.");
                    });
                cmd.CreateCommand($"{command.Name}.del").Hide()
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Delete a message from a command at the specified index.")
                    .Parameter("index", ParameterType.Optional)
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string commandFile = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        if (!File.Exists(commandFile)) return;

                        int i;
                        bool isNumeric = Int32.TryParse(e.Args[0], out i);
                        if (!isNumeric)
                        {
                            await e.Channel.SendMessage($"**{e.Args[0]}** is not a valid index.");
                            return;
                        }

                        var cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(commandFile));
                        if (cmdObj.Messages.Count() < i)
                        {
                            await e.Channel.SendMessage($"**{e.Args[0]}** is not a valid index.");
                            return;
                        }

                        cmdObj.Messages.RemoveAt(i);
                        cmdObj.EditedBy = e.User.Id;
                        cmdObj.EditedOn = DateTime.Now;

                        File.WriteAllText(commandFile, JsonConvert.SerializeObject(cmdObj));

                        await e.Channel.SendMessage($"Deleted message #{i} from `{cmdObj.Name}`.");
                    });
                cmd.CreateCommand($"{command.Name}.info").Hide()
                    .MinPermissions((int)AccessLevel.User)
                    .Description("Get information about this command.")
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string commandFile = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        if (!File.Exists(commandFile)) return;

                        var cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(commandFile));

                        var message = new List<string>();

                        switch (cmdObj.Bind)
                        {
                            case Bind.Global:
                                message.Add($"Info for the global command `{cmdObj.Name}`");
                                break;
                            case Bind.Server:
                                message.Add($"Info for the server command `{cmdObj.Name}`");
                                break;
                            case Bind.Channel:
                                message.Add($"Info for the channel command `{cmdObj.Name}`");
                                break;
                        }
                        message.Add("```erlang");
                        message.Add($"Messages: {cmdObj.Messages.Count}");
                        message.Add($" Created: {e.Server.GetUser(cmdObj.CreatedBy)}");
                        
                        var created = DateTime.Now - cmdObj.CreatedOn;
                        message.Add($"      On: {Math.Round(created.TotalDays, 2)} days ago");

                        message.Add($"  Edited: {e.Server.GetUser(cmdObj.EditedBy)}");

                        var edited = DateTime.Now - cmdObj.EditedOn;
                        message.Add($"      On: {Math.Round(edited.TotalDays, 2)} days ago");

                        message.Add("```");
                        await e.Channel.SendMessage(string.Join("\n", message));
                    });
            });
        }
    }
}