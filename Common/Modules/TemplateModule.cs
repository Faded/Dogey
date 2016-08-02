using Discord;
using Discord.Modules;
using Dogey.Utility;

namespace Dogey.Common.Modules
{
    public class TemplateModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("")
                    .Description("")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("Don't");
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "Module", "Done");
        }
    }
}                  