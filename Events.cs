using Discord;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Dogey.Common.Modules;

namespace Dogey
{
    public class Events
    {
        private static Channel ActivityChannel(Server s)
        {
            string guildConfig = $@"servers\{s.Id}\guild.json";

            if (File.Exists(guildConfig))
            {
                var settings = JsonConvert.DeserializeObject<GuildSettings>(File.ReadAllText(guildConfig));

                if (settings.EnableActivity && settings.ActivityChannel != null)
                {
                    return s.GetChannel((ulong)settings.ActivityChannel);
                }
            }
            return null;
        }

        internal static void MessageRecieved(object s, MessageEventArgs e)
        {
            if (e.Message.IsMentioningMe()) Console.BackgroundColor = ConsoleColor.DarkBlue;
            if (e.Channel.IsPrivate && e.Server == null)
            {
                DogeyConsole.Append("[PM]", ConsoleColor.DarkMagenta);
            }
            else
            {
                DogeyConsole.Append($"[{e.Server.Name} - {e.Channel.Name}]", ConsoleColor.DarkYellow);
            }

            DogeyConsole.Append($"[{e.User.Name}]", ConsoleColor.Yellow);
            DogeyConsole.Append($" {e.Message.RawText}", ConsoleColor.White);

            if (e.Message.Attachments.Count() > 0)
            {
                DogeyConsole.Append($" +{e.Message.Attachments.Count()}", ConsoleColor.Green);
            }

            DogeyConsole.NewLine();
            Console.BackgroundColor = ConsoleColor.Black;
        }
        internal static async void MessageDeleted(object sender, MessageEventArgs e)
        {
            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Message Deleted** {e.User}");
                msg.Add("```erlang");
                msg.Add($"{e.Message.User} #{e.Message.Channel.Name}");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        internal static async void MessageUpdated(object sender, MessageUpdatedEventArgs e)
        {
            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Message Updated** {e.User}");
                msg.Add("```erlang");
                msg.Add($"{e.Before.User} #{e.Channel.Name}");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }

        internal static void ProfileUpdated(object sender, ProfileUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static void UserJoined(object sender, UserEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"has joined the server.");
        }
        internal static void UserLeft(object sender, UserEventArgs e)
        {

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"has left the server.");
        }
        internal static void UserBannned(object sender, UserEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"was banned from the server. ");
            Console.ForegroundColor = ConsoleColor.Green;
        }
        internal static void UserUnbanned(object sender, UserEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"was unbanned from the server. ");
            Console.ForegroundColor = ConsoleColor.Green;
        }
        internal static void UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            if (e.Before.VoiceChannel != null)
            {
                var channel = e.Before.VoiceChannel;
                string tempChannel = $@"servers\{e.Server.Id}\{channel.Id}.voice";

                if (File.Exists(tempChannel) && channel.Users.Count() < 1)
                {
                    channel.Delete();
                    File.Delete(tempChannel);
                }
            }
        }

        internal static void JoinedServer(object sender, ServerEventArgs e)
        {
            string serverFolder = $@"servers\{e.Server.Id}";
            Directory.CreateDirectory(Path.Combine(serverFolder, "commands"));
            Directory.CreateDirectory(Path.Combine(serverFolder, "logs"));

            DogeyConsole.Log(LogSeverity.Info, e.Server.Name, "Joined new server.");
        }
        internal static void ServerUpdated(object sender, ServerUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static void RoleCreated(object sender, RoleEventArgs e)
        {
            throw new NotImplementedException();
        }
        internal static void RoleDeleted(object sender, RoleEventArgs e)
        {
            throw new NotImplementedException();
        }
        internal static void RoleUpdated(object sender, RoleUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static void ChannelCreated(object sender, ChannelEventArgs e)
        {
            throw new NotImplementedException();
        }
        internal static void ChannelDestroyed(object sender, ChannelEventArgs e)
        {
            throw new NotImplementedException();
        }
        internal static void ChannelUpdated(object sender, ChannelUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        internal static void CommandError(object sender, CommandErrorEventArgs e)
        {
            //DogeyConsole.Log(Enum.GetName(typeof(CommandErrorType), e.ErrorType), e.Command.Text, e.Exception.Message);
            //e.Channel.SendMessage(e.Exception.ToString());
        }

    }
}
