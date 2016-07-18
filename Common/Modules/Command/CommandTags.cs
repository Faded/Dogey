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
         ServerId = "%serverid%";

        private const string
         Day1 = "%d%",
         Day2 = "%dd%",
         Day3 = "%ddd%",
         Day4 = "%dddd%",
         Month1 = "%m%",
         Month2 = "%mm%",
         Month3 = "%mmm%",
         Month4 = "%mmmm%",
         Year1 = "%y%",
         Year2 = "%yy%",
         Year3 = "%yyyy%",
         Hour1 = "%h%",
         Hour2 = "%hh%",
         Hour3 = "%24h%",
         Min1 = "%n%",
         Min2 = "%nn%",
         Sec1 = "%s%",
         Sec2 = "%ss%",
         Meridian = "%ampm%";
        
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
                        continue;
                    case UserName:
                        output = output.Replace(tag.Value, e.User.Name);
                        continue;
                    case UserID:
                        output = output.Replace(tag.Value, e.User.Id.ToString());
                        continue;
                    case UserNickMention:
                        output = output.Replace(tag.Value, e.User.NicknameMention);
                        continue;
                    case UserNameMention:
                        output = output.Replace(tag.Value, e.User.Mention);
                        continue;
                    case ChannelName:
                        output = output.Replace(tag.Value, e.Channel.Name);
                        continue;
                    case ChannelID:
                        output = output.Replace(tag.Value, e.Channel.Id.ToString());
                        continue;
                    case ChannelMention:
                        output = output.Replace(tag.Value, e.Channel.Mention);
                        continue;
                    case ServerName:
                        output = output.Replace(tag.Value, e.Server.Name);
                        continue;
                    case ServerId:
                        output = output.Replace(tag.Value, e.Server.Id.ToString());
                        continue;
                }
                switch (tag.Value.ToLower())
                {
                    case Day1:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("d"));
                        continue;
                    case Day2:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("dd"));
                        continue;
                    case Day3:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("ddd"));
                        continue;
                    case Day4:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("dddd"));
                        continue;
                    case Month1:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("M"));
                        continue;
                    case Month2:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("MM"));
                        continue;
                    case Month3:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("MMM"));
                        continue;
                    case Month4:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("MMMM"));
                        continue;
                    case Year1:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("y"));
                        continue;
                    case Year2:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("yy"));
                        continue;
                    case Year3:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("yyyy"));
                        continue;
                    case Hour1:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("h"));
                        continue;
                    case Hour2:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("hh"));
                        continue;
                    case Hour3:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("H"));
                        continue;
                    case Min1:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("m"));
                        continue;
                    case Min2:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("mm"));
                        continue;
                    case Sec1:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("s"));
                        continue;
                    case Sec2:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("ss"));
                        continue;
                    case Meridian:
                        output = output.Replace(tag.Value, DateTime.Now.ToString("tt"));
                        continue;
                }
            }

            return output;
        }
    }
}
