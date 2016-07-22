using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class GuildSettings
    {
        public ulong Id { get; set; }
        public HelpMode HelpMode { get; set; }

        public bool EnableActivity { get; set; }
        public ulong? ActivityChannel { get; set; }

        public bool EnableStar { get; set; }
        public ulong? StarChannel { get; set; }

        public GuildSettings(ulong id)
        {
            Id = id;
            HelpMode = HelpMode.Disabled;
            EnableActivity = false;
            EnableStar = false;
        }
    }
}
