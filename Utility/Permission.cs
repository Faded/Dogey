using Discord;
using Dogey.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Utility
{
    public static class Permission
    {
        public static AccessLevel Get(User u, Channel c)
        {
            if (Program.config.Owner.Contains(u.Id))
                return AccessLevel.BotAdmin;

            if (u.IsBot || u.IsServerDeafened || u.IsServerMuted || u.IsServerSuppressed)
                return AccessLevel.Ignore;

            if (!c.IsPrivate)
            {
                if (u == c.Server.Owner)
                    return AccessLevel.ServerOwner;

                if (u.ServerPermissions.Administrator)
                    return AccessLevel.ServerAdmin;

                if (u.ServerPermissions.KickMembers && u.ServerPermissions.BanMembers)
                    return AccessLevel.ServerAdmin;

                if (u.GetPermissions(c).ManageChannel)
                    return AccessLevel.ChannelAdmin;

                if (u.GetPermissions(c).ManageMessages)
                    return AccessLevel.ChannelMod;
            }
            return AccessLevel.User;
        }
    }
}
