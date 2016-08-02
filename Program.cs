using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using Dogey.Common;
using Dogey.Common.Modules;
using Dogey.Utility;
using System;
using System.IO;
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
                x.HelpMode = HelpMode.Private;
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
            _dogey.Ready += (s, e) =>
            {
                foreach (var server in _dogey.Servers)
                {
                    if (!Directory.Exists($@"servers\{server.Id}"))
                        Directory.CreateDirectory($@"servers\{server.Id}");
                }
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
            _dogey.MessageReceived += Events.MessageRecieved;
            _dogey.MessageDeleted += Events.MessageDeleted;
            _dogey.MessageUpdated += Events.MessageUpdated;

            _dogey.ProfileUpdated += Events.ProfileUpdated;

            _dogey.UserJoined += Events.UserJoined;
            _dogey.UserLeft += Events.UserLeft;
            _dogey.UserBanned += Events.UserBannned;
            _dogey.UserUnbanned += Events.UserUnbanned;
            _dogey.UserUpdated += Events.UserUpdated;
            
            _dogey.JoinedServer += Events.JoinedServer;
            _dogey.ServerUpdated += Events.ServerUpdated;

            _dogey.RoleCreated += Events.RoleCreated;
            _dogey.RoleDeleted += Events.RoleDeleted;
            _dogey.RoleUpdated += Events.RoleUpdated;

            _dogey.ChannelCreated += Events.ChannelCreated;
            _dogey.ChannelDestroyed += Events.ChannelDestroyed;
            _dogey.ChannelUpdated += Events.ChannelUpdated;
        }

        private static void LoadModules()
        {
            _dogey.AddModule<GuildModule>("Guild", ModuleFilter.None);
            _dogey.AddModule<OwnerModule>("Guild", ModuleFilter.None);
            _dogey.AddModule<ModeratorModule>("Moderation", ModuleFilter.None);

            _dogey.AddModule<GamesModule>("General", ModuleFilter.None);
            _dogey.AddModule<SearchModule>("General", ModuleFilter.None);
            _dogey.AddModule<InfoModule>("General", ModuleFilter.None);
            _dogey.AddModule<TempModule>("General", ModuleFilter.None);
            _dogey.AddModule<DogeyModule>("Other", ModuleFilter.None);

            //_dogey.AddModule<CommandModule>("Custom", ModuleFilter.None);
            _dogey.AddModule<CommandModule>("Custom", ModuleFilter.None);
        }
    }
}
