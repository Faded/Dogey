using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using NCalc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Dogey.Common.Modules
{
    public class DogeyModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;
        public Stopwatch timer = new Stopwatch();

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            timer.Start();
            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("doge")
                    .AddCheck((cm, u, ch) => !ch.IsPrivate)
                    .Description("Get a doge.")
                    .Parameter("phrase", ParameterType.Multiple)
                    .Do(async e =>
                    {
                        var r = new Random();
                        string dogeFile = $"servers\\{e.Server.Id}\\{r.Next(10000, 99999)}.png";
                        
                        string dogeText = null;
                        foreach (string arg in e.Args)
                        {
                            if (dogeText == null)
                                dogeText += arg;
                            else
                                dogeText += "/" + arg;
                        }
                        
                        using (WebClient client = new WebClient())
                            client.DownloadFile($"http://dogr.io/wow/{Uri.UnescapeDataString(dogeText)}.png", dogeFile);
                        
                        await e.Channel.SendFile(dogeFile);
                        System.IO.File.Delete(dogeFile);
                    });
                cmd.CreateCommand("evaluate")
                    .AddCheck((cm, u, ch) => !ch.IsPrivate)
                    .Alias(new string[] { "eval" })
                    .Description("Do some math.")
                    .Parameter("Math", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var express = new Expression(e.Args[0]);
                        await e.Channel.SendMessage($"The solution for **{e.Args[0]}** is **{express.Evaluate()}**");
                    });
                cmd.CreateCommand("info")
                    .Description("Get information about Dogey's current session.")
                    .Do(async e =>
                    {
                        var msg = new List<string>();
                        var proc = Process.GetCurrentProcess();

                        string days = $"**{timer.Elapsed.Days}** day";
                        if (days != "1 day") days += "s";
                        string hours = $"**{timer.Elapsed.Hours}** hour";
                        if (hours != "1 hour") hours += "s";
                        string minutes = $"**{timer.Elapsed.Minutes}** minute";
                        if (minutes != "1 minute") minutes += "s";

                        msg.Add("```erlang");
                        msg.Add($"  Uptime: {days} {hours} {minutes}");
                        msg.Add($"  Memory: {(double.Parse(proc.PrivateMemorySize64.ToString()) / 1000).ToString("N2")} kb");
                        msg.Add($"  People: {_dogey.Servers.Count()} servers {_dogey.Servers.Sum(x => x.ChannelCount)} channels {_dogey.Servers.Sum(x => x.UserCount)} users");
                        msg.Add($" Version: {Assembly.GetExecutingAssembly().GetName().Version}");
                        msg.Add("```");

                        await e.Channel.SendMessage(string.Join("\n", msg));
                    });
                cmd.CreateCommand("invite")
                    .Description("Provides a link to invite Dogey to a server.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("https://discordapp.com/oauth2/authorize?client_id=180692566608576512&scope=bot&permissions=67226624");
                    });
                cmd.CreateCommand("wiki")
                    .Description("Provides a link to Dogey's wiki.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("https://github.com/Auxes/Dogey/wiki");
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "DogeyModule", "Done");
        }
    }
}