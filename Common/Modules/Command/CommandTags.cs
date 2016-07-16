using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class CommandTags
    {
        private const string
         UserNick = "%usernick%",
         UserName = "%username%",
         UserID = "%userid%",
         UserNickMention = "%usernickmtn%",
         UserNameMention = "%usernamemtn%",
         ChannelName = "%channelname%",
         ChannelID = "%channelid%",
         ChannelMention = "%channelmtn%",
         ServerName = "%servername%",
         ServerId = "%serverid%",
         Today = "%today%";
        
        public static string Format(string message, CommandEventArgs e)
        {
            Regex r = new Regex(@"%(.+?)%");
            MatchCollection tags = r.Matches(message);
            string output = message;
            
            foreach (Match tag in tags)
            {
                switch (tag.Value.ToLower())
                {
                    case UserNick:
                        output = output.Replace(tag.Value, e.User.Nickname);
                        break;
                    case UserName:
                        output = output.Replace(tag.Value, e.User.Name);
                        break;
                    case UserID:
                        output = output.Replace(tag.Value, e.User.Id.ToString());
                        break;
                    case UserNickMention:
                        output = output.Replace(tag.Value, e.User.NicknameMention);
                        break;
                    case UserNameMention:
                        output = output.Replace(tag.Value, e.User.Mention);
                        break;
                    case ChannelName:
                        output = output.Replace(tag.Value, e.Channel.Name);
                        break;
                    case ChannelID:
                        output = output.Replace(tag.Value, e.Channel.Id.ToString());
                        break;
                    case ChannelMention:
                        output = output.Replace(tag.Value, e.Channel.Mention);
                        break;
                    case ServerName:
                        output = output.Replace(tag.Value, e.Server.Name);
                        break;
                    case ServerId:
                        output = output.Replace(tag.Value, e.Server.Id.ToString());
                        break;
                    case Today:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("d/MM/yy"));
                        break;
                }
            }

            return output;
        }
    }
}
