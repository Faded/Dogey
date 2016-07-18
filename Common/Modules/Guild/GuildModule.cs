using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class GuildModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("activity", cmd =>
            {
                cmd.CreateCommand("channel")
                    .Description("Set the activity channel.")
                    .Parameter("id/mention", ParameterType.Required)
                    .Do(async e =>
                    {
                        await e.User.SendMessage("Don't");
                    });
                cmd.CreateCommand("enable")
                    .Description("Enable logging guild activity to the specified activity channel.")
                    .Do(async e =>
                    {
                        await e.User.SendMessage("Don't");
                    });
                cmd.CreateCommand("disable")
                    .Description("Disable logging guild activity to the specified activity channel.")
                    .Do(async e =>
                    {
                        await e.User.SendMessage("Don't");
                    });
            });

            manager.CreateCommands("star", cmd =>
            {
                cmd.CreateCommand("channel")
                    .Description("Set the star channel.")
                    .Parameter("id/mention", ParameterType.Required)
                    .Do(async e =>
                    {
                        await e.User.SendMessage("Don't");
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "GuildModule", "Loaded.");
        }
    }
}