using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common
{
    public enum AccessLevel
    {
        BotAdmin,
        ServerOwner,
        ServerAdmin,
        ServerMod,
        ChannelAdmin,
        ChannelMod,
        User,
        Ignore
    }
}
