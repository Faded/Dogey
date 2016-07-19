using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using Dogey.Common;
using Dogey.Common.Modules;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    class Program
    {
        public static DiscordClient _dogey { get; set; }
        public static Configuration config = null;
        
        static void Main(string[] args)
        {
            Console.Title = "Dogey";
            Startup.DogeyBanner();

            if (!Startup.ConfigExists())
            {
                Startup.CreateConfig();
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            
            config = new Configuration().FromFile("config\\configuration.json");
            _dogey = new DiscordClient(x =>
            {
                x.AppName = "Dogey";
                x.AppUrl = "https://github.com/Auxes/Dogey";
                x.UsePermissionsCache = true;
                x.EnablePreUpdateEvents = true;
                x.LogLevel = LogSeverity.Info;
            })
            .UsingCommands(x =>
            {
                x.AllowMentionPrefix = false;
                x.HelpMode = HelpMode.Public;
                x.PrefixChar = config.Prefix;
                x.ErrorHandler = Events.CommandError;
            })
            .UsingPermissionLevels((u, c) => (int)Permission.Get(u, c))
            .UsingModules();

            LoadEvents();
            LoadModules();

            _dogey.Log.Message += (s, e) =>
            {
                DogeyConsole.Log(e.Severity, e.Source, e.Message);
            };

            _dogey.ExecuteAndWait(async () =>
            {
                while (true)
                {
                    try {
                        await _dogey.Connect(config.Token.Discord);
                        break;
                    } catch (Exception ex)
                    {
                        _dogey.Log.Error($"Login Failed", ex);
                        DogeyConsole.Write(ex.ToString());
                        await Task.Delay(_dogey.Config.FailedReconnectDelay);
                    }
                }
            });
        }

        private static void LoadEvents()
        {
            _dogey.MessageReceived += Events.OnMessageRecieved;
            _dogey.UserJoined += Events.UserJoined;
            _dogey.UserLeft += Events.UserLeft;
            _dogey.UserBanned += Events.UserBannned;
            _dogey.UserUnbanned += Events.UserUnbanned;
            _dogey.UserUpdated += Events.UserUpdated;
            _dogey.JoinedServer += Events.JoinedServer;
        }

        private static void LoadModules()
        {
            _dogey.AddModule<DogeyModule>("Dogey", ModuleFilter.None);
            _dogey.AddModule<GuildModule>("Guild", ModuleFilter.None);
            _dogey.AddModule<CommandModule>("Command", ModuleFilter.None);
            _dogey.AddModule<AdminModule>("Admin", ModuleFilter.None);
            _dogey.AddModule<GamesModule>("Games", ModuleFilter.None);
            _dogey.AddModule<SearchModule>("Search", ModuleFilter.None);
            _dogey.AddModule<InfoModule>("Info", ModuleFilter.None);
        }
    }
}
