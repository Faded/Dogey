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

namespace Dogey
{
    public class Events
    {
        public static void JoinedServer(object sender, ServerEventArgs e)
        {
            string serverFolder = $@"servers\{e.Server.Id}";
            Directory.CreateDirectory(Path.Combine(serverFolder, "commands"));
            Directory.CreateDirectory(Path.Combine(serverFolder, "logs"));

            DogeyConsole.Log(LogSeverity.Info, e.Server.Name, "Joined new server.");
        }

        public static void OnMessageRecieved(object s, MessageEventArgs e)
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

        public static void UserJoined(object sender, UserEventArgs e)
        {
            string banFile = $@"bans\{e.User.Id}.ban";
            e.Server.DefaultChannel.SendMessage($"Welcome {e.User.Mention} to the server!");
            
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"has joined the server.");
        }
        
        public static void UserLeft(object sender, UserEventArgs e)
        {

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"has left the server.");
        }

        public static void UserUnbanned(object sender, UserEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"was unbanned from the server. ");
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void UserBannned(object sender, UserEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"was banned from the server. ");
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void CommandError(object sender, CommandErrorEventArgs e)
        {
            //DogeyConsole.Log(Enum.GetName(typeof(CommandErrorType), e.ErrorType), e.Command.Text, e.Exception.Message);
            //e.Channel.SendMessage(e.Exception.ToString());
        }

        public static void UserUpdated(object sender, UserUpdatedEventArgs e)
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
    }
}
