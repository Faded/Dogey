using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common
{
    public enum AccessLevel
    {
        Ignore,
        User,
        ChannelMod,
        ChannelAdmin,
        ServerMod,
        ServerAdmin,
        ServerOwner,
        BotAdmin
    }
}
