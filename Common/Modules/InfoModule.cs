using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dogey.Common.Modules
{
    public class InfoModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("serverinfo")
                    .Description("Get info about this server.")
                    .Do(async e =>
                    {
                        var infomsg = new List<string>();

                        infomsg.Add("```erlang");
                        infomsg.Add("  Server: " + e.Server.Name);
                        infomsg.Add("      Id: " + e.Server.Id);
                        infomsg.Add("  Region: " + e.Server.Region.Name);
                        infomsg.Add("   Users: " + e.Server.UserCount);
                        infomsg.Add($"Channels: ({e.Server.TextChannels.Count()})text " +
                                              $"({e.Server.VoiceChannels.Count()})voice " +
                                              $"({e.Server.TextChannels.Count(x => x.Users.Count() < e.Server.Users.Count())})hidden");
                        infomsg.Add($"   Owner: {e.Server.Owner.Name}#{e.Server.Owner.Discriminator}");
                        infomsg.Add("    Icon: " + e.Server.IconUrl);
                        infomsg.Add("   Roles: " + string.Join(", ", e.Server.Roles.Where(x => !x.Name.Contains("@everyone"))));
                        infomsg.Add("```");

                        await e.Channel.SendMessage(string.Join("\n", infomsg));
                    });
                cmd.CreateCommand("userinfo")
                    .Description("Get info about the (optionally) specified user.")
                    .Parameter("user", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        User u = null;
                        string findUser = e.Args[0];
                        if (!string.IsNullOrWhiteSpace(findUser))
                        {
                            if (e.Message.MentionedUsers.Count() == 1)
                            {
                                u = e.Message.MentionedUsers.FirstOrDefault();
                            }
                            else
                            if (e.Server.FindUsers(e.Args[0]).Any())
                            {
                                u = e.Server.FindUsers(e.Args[0]).FirstOrDefault();
                            } else
                            {
                                await e.Channel.SendMessage($"I was unable to find a user like `{findUser}`.");
                                return;
                            }
                        } else
                        {
                            u = e.User;
                        }

                        var infomsg = new List<string>();

                        infomsg.Add("```erlang");

                        if (!string.IsNullOrWhiteSpace(u.Nickname))
                        {
                            infomsg.Add($"       Nick: {u.Nickname}");
                        }

                        infomsg.Add($"       Name: {u.Name}#{u.Discriminator}");
                        infomsg.Add($"         Id: {u.Id}");

                        if (u.CurrentGame != null)
                        {
                            if (!string.IsNullOrWhiteSpace(u.CurrentGame.Value.Url))
                            {
                                infomsg.Add($"  Streaming: {u.CurrentGame.Value.Name} at {u.CurrentGame.Value.Url}");
                            } else
                            {
                                infomsg.Add($"    Playing: {u.CurrentGame.Value.Name}");
                            }
                        }
                        
                        if (u.JoinedAt != null)
                        {
                            var jspan = DateTime.Now - u.JoinedAt;
                            infomsg.Add($"     Joined: {Math.Round(jspan.TotalDays, 1)} days ago ({u.JoinedAt.ToUniversalTime()})");
                        }
                        if (u.LastActivityAt != null)
                        {
                            var aspan = DateTime.Now - DateTime.Parse(u.LastActivityAt.ToString());
                            infomsg.Add($"Last Active: {Math.Round(aspan.TotalDays, 1)} days ago ({DateTime.Parse(u.LastActivityAt.ToString()).ToUniversalTime()})");
                        }
                        if (u.LastOnlineAt != null)
                        {
                            var ospan = DateTime.Now - DateTime.Parse(u.LastOnlineAt.ToString());
                            infomsg.Add($"Last Online: {Math.Round(ospan.TotalDays, 1)} days ago ({DateTime.Parse(u.LastOnlineAt.ToString()).ToUniversalTime()})");
                        }

                        infomsg.Add($"       Icon: {u.AvatarUrl}");
                        infomsg.Add($"      Roles: {string.Join(", ", u.Roles.Where(x => !x.Name.Contains("@everyone")))}");
                        infomsg.Add("```");

                        await e.Channel.SendMessage(string.Join("\n", infomsg));
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "InfoModule", "Done");
        }
    }
}