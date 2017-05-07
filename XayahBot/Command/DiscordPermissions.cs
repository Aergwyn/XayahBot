using Discord;
using Discord.Commands;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public static class DiscordPermissions
    {
        public static bool IsOwner(ICommandContext context)
        {
            return Property.Author.Equals(context.User.ToString());
        }

        public static bool IsOwner(IUser user)
        {
            return Property.Author.Equals(user.ToString());
        }

        public static bool IsMod(ICommandContext context)
        {
            if (context.User is IGuildUser guildUser &&
                (guildUser.GuildPermissions.Has(GuildPermission.Administrator) || guildUser.GuildPermissions.Has(GuildPermission.ManageGuild) ||
                guildUser.GuildPermissions.Has(GuildPermission.ManageRoles) || guildUser.GuildPermissions.Has(GuildPermission.ManageChannels)))
            {
                return true;
            }
            return false;
        }

        public static bool IsOwnerOrMod(ICommandContext context)
        {
            if (IsOwner(context) || IsMod(context))
            {
                return true;
            }
            return false;
        }
    }
}
