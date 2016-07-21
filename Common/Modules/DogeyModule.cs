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
using System.Text;
using System.Threading.Tasks;

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
                            {
                                dogeText += arg;
                            }
                            else
                            {
                                dogeText += "/" + arg;
                            }
                        }
                        
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile($"http://dogr.io/wow/{Uri.UnescapeDataString(dogeText)}.png", dogeFile);
                        }

                        await e.Channel.SendFile(dogeFile);
                        System.IO.File.Delete(dogeFile);
                    });
                cmd.CreateCommand("evaluate")
                    .Description("Do some math.")
                    .Parameter("Math", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var express = new Expression(e.Args[0]);

                        await e.Channel.SendMessage($"The solution for **{e.Args[0]}** is **{express.Evaluate()}**");
                    });
                cmd.CreateCommand("uptime")
                    .Description("Shows how long Dogey has been online.")
                    .Do(async e =>
                    {
                        string days = $"**{timer.Elapsed.Days}** day";
                        if (days != "1 day") days += "s";
                        string hours = $"**{timer.Elapsed.Hours}** hour";
                        if (hours != "1 hour") hours += "s";
                        string minutes = $"**{timer.Elapsed.Minutes}** minute";
                        if (minutes != "1 minute") minutes += "s";

                        await e.Channel.SendMessage($"I have been online for {days} {hours} {minutes}.");
                    });
                cmd.CreateCommand("version")
                    .Description("Shows what version Dogey is currently on.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage($"I am running version **{Assembly.GetExecutingAssembly().GetName().Version}**.");
                    });
                cmd.CreateCommand("invite")
                    .Description("Provides a link to invite Dogey to a server.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("https://discordapp.com/oauth2/authorize?client_id=180692566608576512&scope=bot&permissions=67226624");
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "DogeyModule", "Loaded.");
        }
    }
}