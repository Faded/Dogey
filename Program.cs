﻿using Discord;
using Discord.Commands;
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
                x.MessageCacheSize = 0;
                x.UsePermissionsCache = true;
                x.EnablePreUpdateEvents = true;
                x.LogLevel = LogSeverity.Debug;
            })
            .UsingCommands(x =>
            {
                x.AllowMentionPrefix = false;
                x.HelpMode = HelpMode.Public;
                x.PrefixChar = config.Prefix;
            })
            .UsingModules();

            _dogey.MessageReceived += Events.OnMessageRecieved;

            _dogey.AddModule<DogeyModule>("Dogey", ModuleFilter.None);
            _dogey.AddModule<CustomModule>("Custom", ModuleFilter.None);
            //_dogey.AddModule<UsageModule>("Usage", ModuleFilter.None);

            _dogey.ExecuteAndWait(async () =>
            {
                while (true)
                {
                    try {
                        await _dogey.Connect(config.Token);
                        DogeyConsole.Write("Connected to Discord using bot token.\n");
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

    }
}
