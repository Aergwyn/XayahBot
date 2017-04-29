using Discord;
using Discord.Commands;
using XayahBot.Utility;

namespace XayahBot.Command.System
{
    public class Permission
    {
        public bool IsAdmin(ICommandContext context)
        {
            return Property.Author.Equals(context.User.ToString());
        }

        public bool IsMod(ICommandContext context)
        {
            if (context.User is IGuildUser guildUser &&
                (guildUser.GuildPermissions.Has(GuildPermission.Administrator) || guildUser.GuildPermissions.Has(GuildPermission.ManageGuild) ||
                guildUser.GuildPermissions.Has(GuildPermission.ManageRoles) || guildUser.GuildPermissions.Has(GuildPermission.ManageChannels)))
            {
                return true;
            }
            return false;
        }

        public bool IsAdminOrMod(ICommandContext context)
        {
            if (this.IsAdmin(context) || this.IsMod(context))
            {
                return true;
            }
            return false;
        }
    }
}
