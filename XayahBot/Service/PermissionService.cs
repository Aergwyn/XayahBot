using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Discord.Commands;
using XayahBot.Utility;

namespace XayahBot.Service
{
    public static class PermissionService
    {
        public static bool IsAdmin(CommandContext context)
        {
            return Property.Author.Equals(context.User.ToString());
        }

        public static bool IsMod(CommandContext context)
        {
            if (Property.ConfigMods.Value.Split(',').FirstOrDefault(x => x.Equals(context.User.ToString())) != null)
            {
                return true;
            }
            return false;
        }

        public static bool IsAdminOrMod(CommandContext context)
        {
            if (IsAdmin(context) || IsMod(context))
            {
                return true;
            }
            return false;
        }

        //

        public static bool AddMod(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && Regex.IsMatch(name, "^.+#[0-9]{4}$") && !Property.ConfigMods.Value.Split(',').Contains(name))
            {
                List<string> mods = Property.ConfigMods.Value.Split(',').ToList();
                mods.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                mods.Add(name);
                Property.ConfigMods.Value = string.Join(",", mods);
                return true;
            }
            return false;
        }

        public static bool RemoveMod(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && Regex.IsMatch(name, "^.+#[0-9]{0,4}$"))
            {
                List<string> mods = Property.ConfigMods.Value.Split(',').ToList();
                mods.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                if (mods.Count() > 0)
                {
                    int preRemoveCount = mods.Count();
                    List<string> matches = mods.Where(x => x.ToLower().Contains(name.ToLower())).ToList();
                    foreach (string mod in matches)
                    {
                        mods.Remove(mod);
                    }
                    if (mods.Count() != preRemoveCount)
                    {
                        Property.ConfigMods.Value = string.Join(",", mods);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
