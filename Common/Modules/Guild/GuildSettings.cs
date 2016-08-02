using Discord.Commands;

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
