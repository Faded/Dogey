﻿using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("createcommand")
                    .Description("Create a new custom command.")
                    .Alias(new string[] { "createcmd" })
                    .Parameter("name", ParameterType.Required)
                    .Parameter("message", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        string serverFolder = $@"servers\{e.Server.Id}\commands";

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
                            CreatedOn = DateTime.Now
                        };
                        command.Messages.Add(e.Args[1]);

                        string json = JsonConvert.SerializeObject(command);
                        File.Create(Path.Combine(serverFolder, command.Name + ".doge")).Close();
                        File.WriteAllText(Path.Combine(serverFolder, command.Name + ".doge"), json);

                        CreateCommand(manager, command);

                        var msg = await e.Channel.SendMessage($"The command `{command.Name}` has been created.");
                        await Task.Delay(10000);
                        await e.Message.Delete();
                        await msg.Delete();
                    });
                cmd.CreateCommand("deletecommand")
                    .Description("Delete an existing custom command.")
                    .Alias(new string[] { "deletecmd", "delcmd" })
                    .Parameter("name", ParameterType.Required)
                    .Do(async e =>
                    {
                        if (!e.User.ServerPermissions.Administrator) return;
                        string commandFile = $@"servers\{e.Server.Id}\commands\{e.Args[0]}.doge";

                        if (File.Exists(commandFile))
                        {
                            File.Move(commandFile, commandFile.Replace($".doge", ".del"));
                        }
                        else
                        {
                            await e.Channel.SendMessage($"{e.Args[0]} is not an existing command.");
                        }

                        var msg = await e.Channel.SendMessage($"The command `{e.Args[0]}` has been deleted.");
                        await Task.Delay(10000);
                        await e.Message.Delete();
                        await msg.Delete();
                    });
                cmd.CreateCommand("commands")
                    .Description("Displays a list of all available custom commands for this server.")
                    .Alias(new string[] { "cmds" })
                    .Do(async e =>
                    {
                        string serverFolder = $@"servers\{e.Server.Id}\commands\";

                        var commands = new List<string>();
                        var dir = new DirectoryInfo(serverFolder);
                        var commandFiles = dir.GetFiles("*.doge");
                        foreach (FileInfo file in commandFiles)
                        {
                            commands.Add(file.Name.Replace(".doge", ""));
                        }

                        await e.Channel.SendMessage($"**Commands:**\n{string.Join(", ", commands)}");
                    });
                cmd.CreateCommand("*.del")
                    .Description("Delete a message from a command at the specified index.")
                    .Parameter("index", ParameterType.Optional)
                    .Do(e => { return; });
                cmd.CreateCommand("*.add")
                    .Description("Add a new message to the custom command.")
                    .Parameter("Message", ParameterType.Unparsed)
                    .Do(e => { return; });
                cmd.CreateCommand("*.edit")
                    .Description("Edit a message's content.")
                    .Parameter("index", ParameterType.Required)
                    .Do(e => { return; });
                cmd.CreateCommand("*.raw")
                    .Description("View a message's raw text.")
                    .Parameter("index", ParameterType.Required)
                    .Do(e => { return; });
                cmd.CreateCommand("*.count")
                    .Description("Get the total number of messages saved in this command.")
                    .Do(e => { return; });
                cmd.CreateCommand("*.info")
                    .Description("Get information about this command.")
                    .Do(e => { return; });
            });

            LoadExistingCommands(manager);

            DogeyConsole.Log(LogSeverity.Info, "CommandModule", "Loaded.");
        }

        public void LoadExistingCommands(ModuleManager manager)
        {
            int servers = 0;
            int commands = 0;
            if (!Directory.Exists("servers")) Directory.CreateDirectory("servers");
            foreach (string folder in Directory.GetDirectories("servers"))
            {
                foreach (string file in Directory.GetFiles(Path.Combine(folder, "commands")))
                {
                    var cmd = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(file));

                    CreateCommand(manager, cmd);
                    commands++;
                }
                servers++;
            }

            DogeyConsole.Log(LogSeverity.Info, "CommandModule", $"Loaded {commands} command(s) for {servers} server(s).");
        }

        public void CreateCommand(ModuleManager manager, CommandInfo command)
        {
            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand(command.Name)
                    .Description("Create a new custom command.")
                    .Parameter("index", ParameterType.Optional)
                    .Do(async e =>
                    {
                        string commandFile = $@"servers\{e.Server.Id}\commands\{e.Command.Text}.doge";
                        if (!File.Exists(commandFile)) return;

                        var cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(commandFile));

                        switch (cmdObj.Messages.Count)
                        {
                            case 0:
                                await e.Channel.SendMessage($"This command has no stored messages, add some with `{cmdObj.Name}.add [msg]`.");
                                return;
                            case 1:
                                await e.Channel.SendMessage(CommandTags.Format(cmdObj.Messages[0], e));
                                return;
                            default:
                                break;
                        }
                        
                        int i;
                        bool isNumeric = Int32.TryParse(e.Args[0], out i);
                        if (isNumeric)
                        {
                            await e.Channel.SendMessage($"{i}. {CommandTags.Format(cmdObj.Messages[i], e)}");
                            return;
                        }
                        
                        var r = new Random();
                        string message = cmdObj.Messages[r.Next(0, cmdObj.Messages.Count())];
                        await e.Channel.SendMessage($"{cmdObj.Messages.IndexOf(message)}. {CommandTags.Format(message, e)}");
                    });
                cmd.CreateCommand($"{command.Name}.raw").Hide()
                    .Description("View a message's raw text.")
                    .Parameter("index", ParameterType.Required)
                    .Do(async e =>
                    {
                        string commandFile = $@"servers\{e.Server.Id}\commands\{e.Command.Text}.doge";
                        if (!File.Exists(commandFile)) return;

                        var cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(commandFile));

                        switch (cmdObj.Messages.Count)
                        {
                            case 0:
                                await e.Channel.SendMessage($"This command has no stored messages, add some with `{cmdObj.Name}.add [msg]`.");
                                return;
                            case 1:
                                await e.Channel.SendMessage(cmdObj.Messages[0]);
                                return;
                            default:
                                break;
                        }

                        int i;
                        bool isNumeric = Int32.TryParse(e.Args[0], out i);
                        if (isNumeric)
                        {
                            await e.Channel.SendMessage($"{i}. {CommandTags.Format(cmdObj.Messages[i], e)}");
                        } else
                        {
                            await e.Channel.SendMessage($"**{e.Args[0]}** is not a valid index.");
                        }
                    });
                cmd.CreateCommand($"{command.Name}.add").Hide()
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

                        File.WriteAllText(commandFile, JsonConvert.SerializeObject(cmdObj));

                        await e.Channel.SendMessage($"Deleted message #{i} from `{cmdObj.Name}`.");
                    });
                cmd.CreateCommand($"{command.Name}.count").Hide()
                    .Description("Get the total number of messages saved in this command.")
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string commandFile = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        if (!File.Exists(commandFile)) return;

                        var cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(commandFile));

                        await e.Channel.SendMessage($"`{cmdObj.Name}` currently contains **{cmdObj.Messages.Count()}** message(s).");
                    });
                cmd.CreateCommand($"{command.Name}.info").Hide()
                    .Description("Get information about this command.")
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string commandFile = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        if (!File.Exists(commandFile)) return;

                        var cmdObj = JsonConvert.DeserializeObject<CommandInfo>(File.ReadAllText(commandFile));

                        string message;
                        var span = DateTime.Now - cmdObj.CreatedOn;

                        double total = span.TotalDays;
                        if (total >= 1)
                        {
                            message = $"The command `{cmdObj.Name}` was created " +
                                      $"**{total.ToString("#.0")}** day(s) ago " +
                                      $"by **{e.Server.GetUser(cmdObj.CreatedBy).Name}**.";
                        }
                        else
                        {
                            total = span.TotalHours;
                            if (total >= 1)
                            {
                                message = $"The command `{cmdObj.Name}` was created " +
                                          $"**{total.ToString("#.0")}** hour(s) ago " +
                                          $"by **{e.Server.GetUser(cmdObj.CreatedBy).Name}**.";
                            }
                            else
                            {
                                total = span.TotalMinutes;
                                if (total >= 1)
                                {
                                    message = $"The command `{cmdObj.Name}` was created " +
                                              $"**{total.ToString("#.0")}** minute(s) ago " +
                                              $"by **{e.Server.GetUser(cmdObj.CreatedBy).Name}**.";
                                }
                                else
                                {
                                    message = $"The command `{cmdObj.Name}` was created " +
                                              $"less than a minute ago by **{e.Server.GetUser(cmdObj.CreatedBy).Name}**.";
                                }
                            }
                        }

                        await e.Channel.SendMessage(message);
                    });
            });
        }
    }
}