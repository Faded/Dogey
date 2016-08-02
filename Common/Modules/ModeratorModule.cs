using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    class ModeratorModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;
            
            manager.CreateCommands("clean", cmd =>
            {
                cmd.CreateCommand("all")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Delete the specified number of messages from the channel.")
                    .Parameter("amount", ParameterType.Required)
                    .Do(async e =>
                    {
                        int amount;
                        if (Int32.TryParse(e.Args[0], out amount))
                        {
                            if (amount > 50) amount = 50;

                            IEnumerable<Message> msgs;
                            if (e.Channel.Messages.Count() < amount)
                                msgs = await e.Channel.DownloadMessages(amount);
                            else
                                msgs = e.Channel.Messages.OrderByDescending(x => x.Timestamp).Take(amount);
                            
                            int deletedMessages = 0;
                            foreach (Message msg in msgs)
                            {
                                await msg.Delete();
                                deletedMessages++;
                            }

                            var message = await e.Channel.SendMessage($"Deleted **{deletedMessages}** message(s).");
                            await Task.Delay(5000);
                            await e.Message.Delete();
                            await message.Delete();
                        } else
                        {
                            throw new Exception($"`{e.Args[0]}` is not a valid number.");
                        }
                    });
                cmd.CreateCommand("contains")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Delete messages that contain the provided text.")
                    .Parameter("string", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Args[0]))
                        {
                            IEnumerable<Message> msgs;
                            if (e.Channel.Messages.Count() < 25)
                                msgs = await e.Channel.DownloadMessages(25);
                            else
                                msgs = e.Channel.Messages.OrderByDescending(x => x.Timestamp).Take(25);

                            int deletedMessages = 0;
                            foreach (Message msg in msgs)
                            {
                                if (msg.RawText.Contains(e.Args[0]))
                                {
                                    await msg.Delete();
                                    deletedMessages++;
                                }
                            }

                            var message = await e.Channel.SendMessage($"Deleted **{deletedMessages}** message(s) containing `{e.Args[0]}`.");
                            await Task.Delay(5000);
                            await e.Message.Delete();
                            await message.Delete();
                        } else
                        {
                            throw new Exception("You must provide a valid string.");
                        }
                    });
                cmd.CreateCommand("user")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Delete messages sent by the provided user or users.")
                    .Parameter("users", ParameterType.Multiple)
                    .Do(async e =>
                    {
                        if (e.Args.Count() > 0)
                        {
                            IEnumerable<Message> msgs;
                            if (e.Channel.Messages.Count() < 25)
                                msgs = await e.Channel.DownloadMessages(25);
                            else
                                msgs = e.Channel.Messages.OrderByDescending(x => x.Timestamp).Take(25);

                            int deletedMessages = 0;
                            foreach (Message msg in msgs)
                            {
                                if (e.Message.MentionedUsers.Any(x => x.Id == msg.User.Id))
                                {
                                    await msg.Delete();
                                    deletedMessages++;
                                }
                            }

                            var message = await e.Channel.SendMessage($"Deleted **{deletedMessages}** message(s) by `{string.Join(", ", e.Message.MentionedUsers)}`.");
                            await Task.Delay(5000);
                            await e.Message.Delete();
                            await message.Delete();
                        }
                        else
                        {
                            throw new Exception("You must provide at least one user.");
                        }
                    });
                cmd.CreateCommand("bots")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Delete messages sent by bots.")
                    .Do(async e =>
                    {
                        IEnumerable<Message> msgs;
                        if (e.Channel.Messages.Count() < 25)
                            msgs = await e.Channel.DownloadMessages(25);
                        else
                            msgs = e.Channel.Messages.OrderByDescending(x => x.Timestamp).Take(25);

                        int deletedMessages = 0;
                        foreach (Message msg in msgs)
                        {
                            if (msg.User.IsBot)
                            {
                                await msg.Delete();
                                deletedMessages++;
                            }
                        }

                        var message = await e.Channel.SendMessage($"Deleted **{deletedMessages}** bot message(s).");
                        await Task.Delay(5000);
                        await e.Message.Delete();
                        await message.Delete();
                    });
                cmd.CreateCommand("files")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("Delete messages that have files attached.")
                    .Do(async e =>
                    {
                        IEnumerable<Message> msgs;
                        if (e.Channel.Messages.Count() < 25)
                            msgs = await e.Channel.DownloadMessages(25);
                        else
                            msgs = e.Channel.Messages.OrderByDescending(x => x.Timestamp).Take(25);

                        int deletedMessages = 0;
                        foreach (Message msg in msgs)
                        {
                            if (msg.Attachments.Count() > 0)
                            {
                                await msg.Delete();
                                deletedMessages++;
                            }
                        }

                        var message = await e.Channel.SendMessage($"Deleted **{deletedMessages}** message(s) with attachments.");
                        await Task.Delay(5000);
                        await e.Message.Delete();
                        await message.Delete();
                    });
                cmd.CreateCommand("embeds")
                    .MinPermissions((int)AccessLevel.ChannelMod)
                    .Description("")
                    .Parameter("condition", ParameterType.Required)
                    .Do(async e =>
                    {
                        IEnumerable<Message> msgs;
                        if (e.Channel.Messages.Count() < 25)
                            msgs = await e.Channel.DownloadMessages(25);
                        else
                            msgs = e.Channel.Messages.OrderByDescending(x => x.Timestamp).Take(25);

                        int deletedMessages = 0;
                        foreach (Message msg in msgs)
                        {
                            if (msg.Embeds.Count() > 0)
                            {
                                await msg.Delete();
                                deletedMessages++;
                            }
                        }

                        var message = await e.Channel.SendMessage($"Deleted **{deletedMessages}** message(s) with attachments.");
                        await Task.Delay(5000);
                        await e.Message.Delete();
                        await message.Delete();
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "ModeratorModule", "Done");
        }
    }
}