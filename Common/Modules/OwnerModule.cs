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
                    .Parameter("type", ParameterType.Optional)
                    .Do(async e =>
                    {
                        string fileName;
                        if (string.IsNullOrWhiteSpace(e.Args[1]))
                            fileName = $"{e.Args[0]}.jpg";
                        else
                            fileName = $"{e.Args[0]}.{e.Args[1]}";

                        string avatarFile = $@"config\avatars\{fileName}";

                        if (!File.Exists(avatarFile))
                        {
                            await e.Channel.SendMessage($"The file `{fileName}` does not exist.");
                        } else
                        {
                            await _dogey.CurrentUser.Edit(avatar: File.Open(avatarFile, FileMode.Open));
                            await e.Channel.SendMessage($"I changed my avatar to `{fileName}`.");
                        }
                    });
                cmd.CreateCommand("avatars")
                    .MinPermissions((int)AccessLevel.BotAdmin)
                    .Description("List all of the available avatars.")
                    .Do(async e =>
                    {
                        string avatarFile = @"config\avatars";

                        var files = Directory.GetFiles(avatarFile);

                        var avatars = new List<string>();
                        for (int i = 0; i < files.Count(); i++)
                            avatars.Add(Path.GetFileNameWithoutExtension(files[i]).ToLower());
                        
                        await e.Channel.SendMessage($"```erlang\nAvatars: {string.Join(", ", avatars)}```");
                    });
                cmd.CreateCommand("setgame")
                    .MinPermissions((int)AccessLevel.BotAdmin)
                    .Description("Change dogey's game.")
                    .Parameter("game", ParameterType.Required)
                    .Do(async e =>
                    {
                        if (string.IsNullOrWhiteSpace(e.Args[0]))
                        {
                            await e.Channel.SendMessage($"I can't play that game...");
                        }
                        else
                        {
                            _dogey.SetGame(e.Args[0]);
                            await e.Channel.SendMessage($"I am now playing **{e.Args[0]}**.");
                        }
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "OwnerModule", "Done");
        }
    }
}                  