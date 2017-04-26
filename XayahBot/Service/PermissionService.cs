using Discord.Commands;
using XayahBot.Utility;
using Discord;

namespace XayahBot.Service
{
    public static class PermissionService
    {
        public static bool IsAdmin(ICommandContext context)
        {
            return Property.Author.Equals(context.User.ToString());
        }

        public static bool IsMod(ICommandContext context)
        {
            IGuildUser guildUser = context.User as IGuildUser;
            if (guildUser != null && (guildUser.GuildPermissions.Has(GuildPermission.Administrator) || guildUser.GuildPermissions.Has(GuildPermission.ManageGuild) ||
                guildUser.GuildPermissions.Has(GuildPermission.ManageRoles) || guildUser.GuildPermissions.Has(GuildPermission.ManageChannels)))
            {
                return true;
            }
            return false;
        }

        public static bool IsAdminOrMod(ICommandContext context)
        {
            if (IsAdmin(context) || IsMod(context))
            {
                return true;
            }
            return false;
        }
    }
}
