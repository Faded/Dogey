using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class OwnerModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("setavatar")
                    .MinPermissions((int)AccessLevel.BotAdmin)
                    .Description("Change dogey's avatar.")
                    .Parameter("file", ParameterType.Required)
                    .Do(async e =>
                    {
                        string avatarFile = $@"config\avatars\{e.Args[0]}";

                        if (!File.Exists(avatarFile))
                        {
                            await e.Channel.SendMessage($"The file `{e.Args[0]}` does not exist.");
                        } else
                        {
                            await _dogey.CurrentUser.Edit(avatar: File.Open(avatarFile, FileMode.Open));
                            await e.Channel.SendMessage($"I changed my avatar to `{e.Args[0]}`.");
                        }
                    });
                //cmd.CreateCommand("setgame")
                //    .MinPermissions((int)AccessLevel.BotAdmin)
                //    .Description("Change dogey's game.")
                //    .Parameter("game", ParameterType.Required)
                //    .Do(async e =>
                //    {
                //        if (string.IsNullOrWhiteSpace(e.Args[0]))
                //        {
                //            await e.Channel.SendMessage($"I can't play that game...");
                //        } else
                //        {
                //            await _dogey.CurrentUser.Edit(;
                //            await e.Channel.SendMessage($"I can't play that game...");
                //        }
                //    });
            });

            DogeyConsole.Log(LogSeverity.Info, "OwnerModule", "Done");
        }
    }
}                  