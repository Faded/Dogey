using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class CommandInfo
    {
        public DateTime Timestamp { get; set; }
        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
    }
}
