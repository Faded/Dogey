﻿using Discord;
using Discord.WebSocket;
using Dogey.Services;
using System.Threading.Tasks;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;

        public async Task Start()
        {
            PrettyConsole.NewLine("===   Dogey   ===");
            PrettyConsole.NewLine();

            Configuration.EnsureExists();

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 10000
            });

            _client.Log += (l)
                => Task.Run(()
                => PrettyConsole.Log(l.Severity, l.Source, l.Exception?.ToString() ?? l.Message));

            await _client.LoginAsync(TokenType.Bot, Configuration.Load().Token.Discord);
            await _client.StartAsync();

            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);

            await Task.Delay(-1);
        }
    }
}