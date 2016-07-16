using Discord;
using Discord.Commands;
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
    public class UsageModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("usage", cmd =>
            {
                cmd.CreateCommand("")
                    .Description("Provide the user with information about the usage command.")
                    .Do(async e =>
                    {
                        await e.User.SendMessage("Don't");
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "UsageModule", "Loaded.");
        }
    }
}