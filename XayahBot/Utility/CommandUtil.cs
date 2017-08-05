using System.Collections.Generic;
using Discord.Commands;

namespace XayahBot.Utility
{
    public static class CommandUtil
    {
        public static bool HasRequireRole<T>(CommandInfo commandInfo) where T : PreconditionAttribute
        {
            T match = GetPrecondition<T>(commandInfo);
            if (match != null)
            {
                return true;
            }
            return false;
        }

        public static T GetPrecondition<T>(CommandInfo commandInfo) where T : PreconditionAttribute
        {
            T match = GetPrecondition<T>(commandInfo.Preconditions);
            if (match == null)
            {
                match = GetPrecondition<T>(commandInfo.Module.Preconditions);
            }
            return match;
        }

        public static T GetPrecondition<T>(IReadOnlyList<PreconditionAttribute> preconditions) where T : PreconditionAttribute
        {
            T match = default(T);
            foreach (PreconditionAttribute attribute in preconditions)
            {
                if (attribute is T)
                {
                    match = attribute as T;
                    break;
                }
            }
            return match;
        }
    }
}
