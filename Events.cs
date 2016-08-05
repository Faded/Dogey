using Discord;
using Discord.Commands;
using Dogey.Common.Modules;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            } else {
                //File.Create(guildConfig).Close();
                //File.WriteAllText(guildConfig, JsonConvert.SerializeObject(new GuildSettings(s.Id), Formatting.Indented));
            }
            return null;
        }
        
        internal static void MessageRecieved(object s, MessageEventArgs e)
        {
            if (e.Message.IsMentioningMe()) Console.BackgroundColor = ConsoleColor.DarkBlue;
            if (e.Channel.IsPrivate && e.Server == null)
                DogeyConsole.Append("[PM]", ConsoleColor.DarkMagenta);
            else
                DogeyConsole.Append($"[{e.Server.Name} - {e.Channel.Name}]", ConsoleColor.DarkYellow);
            
            DogeyConsole.Append($"[{e.User.Name}]", ConsoleColor.Yellow);
            DogeyConsole.Append($" {e.Message.RawText}", ConsoleColor.White);

            if (e.Message.Attachments.Count() > 0)
                DogeyConsole.Append($" +{e.Message.Attachments.Count()}", ConsoleColor.Green);
            
            DogeyConsole.NewLine();
            Console.BackgroundColor = ConsoleColor.Black;
        }
        internal static async void MessageDeleted(object sender, MessageEventArgs e)
        {
            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Message Deleted** :wastebasket:");
                msg.Add("```erlang");
                msg.Add($"{e.Message.User} ({e.Message.Id}) #{e.Message.Channel.Name}");
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

        internal static async void ProfileUpdated(object sender, ProfileUpdatedEventArgs e)
        {
            await Task.Delay(1);
        }

        internal static async void UserJoined(object sender, UserEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"has joined the server.");

            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                if (e.User.IsBot)
                    msg.Add($"**Bot Joined** :robot:");
                else
                    msg.Add($"**User Joined** :chart_with_upwards_trend:");
                msg.Add("```erlang");
                msg.Add($"{e.User} ({e.User.Id})");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        internal static async void UserLeft(object sender, UserEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"has left the server.");

            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                if (e.User.IsBot)
                    msg.Add($"**Bot Left** :robot:");
                else
                    msg.Add($"**User Left** :chart_with_downwards_trend:");
                msg.Add("```erlang");
                msg.Add($"{e.User} ({e.User.Id})");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        internal static async void UserBannned(object sender, UserEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"was banned from the server. ");
            Console.ForegroundColor = ConsoleColor.Green;

            var activity = ActivityChannel(e.Server);
            
            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**User Banned** :hammer:");
                msg.Add("```erlang");
                msg.Add($"{e.User} ({e.User.Id})");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        internal static async void UserUnbanned(object sender, UserEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"was unbanned from the server. ");
            Console.ForegroundColor = ConsoleColor.Green;

            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**User Unbanned** :ok_hand:");
                msg.Add("```erlang");
                msg.Add($"{e.User} ({e.User.Id})");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        internal static async void UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            if (e.Before.VoiceChannel != null)
            {
                var channel = e.Before.VoiceChannel;
                string tempChannel = $@"servers\{e.Server.Id}\{channel.Id}.voice";

                if (File.Exists(tempChannel) && channel.Users.Count() < 1)
                {
                    await channel.Delete();
                    File.Delete(tempChannel);
                }
            }
            
            var activity = ActivityChannel(e.Server);
            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**User Updated**");
                msg.Add("```erlang");
                msg.Add($"{e.Before} ({e.Before.Id})");

                if (e.Before.Name != e.After.Name)
                    msg.Add($"{e.Before.Name} => {e.After.Name}");
                if (e.Before.Nickname != e.After.Nickname)
                    msg.Add($"{e.Before.Name} => {e.After.Name}");
                if (e.Before.AvatarUrl != e.After.AvatarUrl)
                    msg.Add($"{e.Before.AvatarUrl} => {e.After.AvatarUrl}");

                msg.Add("```");

                if (msg.Count() > 4)
                    await activity.SendMessage(string.Join("\n", msg));
            }
        }

        internal static async void JoinedServer(object sender, ServerEventArgs e)
        {
            string serverFolder = $@"servers\{e.Server.Id}";
            Directory.CreateDirectory(Path.Combine(serverFolder, "commands"));
            Directory.CreateDirectory(Path.Combine(serverFolder, "logs"));

            DogeyConsole.Log(LogSeverity.Info, e.Server.Name, "Joined new server. :OhMyDoge:");

            var ownerGuild = Program._dogey.GetServer(Program.config.OwnerGuild);
            if (ownerGuild != null)
            {
                var activity = ActivityChannel(ownerGuild);

                if (activity != null)
                {
                    var msg = new List<string>();
                    msg.Add($"**Joined Server** :raised_hands:");
                    msg.Add("```erlang");
                    msg.Add($"{e.Server.Name} ({e.Server.Id})");
                    msg.Add($"{e.Server.Owner} ({e.Server.Owner.Id})");
                    msg.Add("```");

                    await activity.SendMessage(string.Join("\n", msg));
                }
            }
        }
        internal static async void ServerUpdated(object sender, ServerUpdatedEventArgs e)
        {
            var activity = ActivityChannel(e.After);
            
            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Server Updated**");
                msg.Add("```erlang");

                if (e.Before.Name != e.After.Name)
                    msg.Add($"{e.Before.Name} => {e.After.Name}");
                if (e.Before.Region != e.After.Region)
                    msg.Add($"{e.Before.Region} => {e.After.Region}");
                
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }

        internal static async void RoleCreated(object sender, RoleEventArgs e)
        {
            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Role Created** :new:");
                msg.Add("```erlang");
                msg.Add($"{e.Role.Name} ({e.Role.Id})");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        internal static async void RoleDeleted(object sender, RoleEventArgs e)
        {
            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Role Deleted**  :wastebasket:");
                msg.Add("```erlang");
                msg.Add($"{e.Role.Name} ({e.Role.Id})");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        internal static async void RoleUpdated(object sender, RoleUpdatedEventArgs e)
        {
            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Role Updated**");
                msg.Add("```erlang");
                msg.Add($"{e.Before.Name} ({e.Before.Id})");

                if (e.Before.Name != e.After.Name)
                    msg.Add($"{e.Before.Name} => {e.After.Name}");
                if (e.Before.Color != e.After.Color)
                    msg.Add($"{e.Before.Color} => {e.After.Color}");

                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }

        internal static async void ChannelCreated(object sender, ChannelEventArgs e)
        {
            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Channel Created** :new:");
                msg.Add("```erlang");
                msg.Add($"{e.Channel.Name} ({e.Channel.Id})");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        internal static async void ChannelDestroyed(object sender, ChannelEventArgs e)
        {
            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Channel Deleted** :wastebasket:");
                msg.Add("```erlang");
                msg.Add($"{e.Channel.Name} ({e.Channel.Id})");
                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        internal static async void ChannelUpdated(object sender, ChannelUpdatedEventArgs e)
        {
            var activity = ActivityChannel(e.Server);

            if (activity != null)
            {
                var msg = new List<string>();
                msg.Add($"**Channel Updated**");
                msg.Add("```erlang");
                msg.Add($"{e.Before.Name} ({e.Before.Id})");

                if (e.Before.Name != e.After.Name)
                    msg.Add($"{e.Before.Name} => {e.After.Name}");
                if (e.Before.Topic != e.After.Topic)
                    msg.Add($"{e.Before.Topic} => {e.After.Topic}");

                msg.Add("```");

                await activity.SendMessage(string.Join("\n", msg));
            }
        }
        
        internal static async void CommandError(object sender, CommandErrorEventArgs e)
        {
            switch (e.ErrorType)
            {
                case CommandErrorType.Exception:
                    await e.Channel.SendMessage(e.Exception.GetBaseException().Message);
                    break;
                case CommandErrorType.BadArgCount:
                    break;
                case CommandErrorType.BadPermissions:
                    break;
                case CommandErrorType.InvalidInput:
                    break;
                case CommandErrorType.UnknownCommand:
                    break;
            }
        }
    }
}
