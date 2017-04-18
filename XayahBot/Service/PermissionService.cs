using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Discord.Commands;
using XayahBot.Utility;
using System;

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
            if (Property.CfgMods.Value.Split(',').FirstOrDefault(x => x.Equals(context.User.ToString())) != null)
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

        public static int ToggleMod(string name)
        {
            return ToggleValue(Property.CfgMods, name);
        }

        public static int ToggleIgnore(string name)
        {
            return ToggleValue(Property.CfgIgnore, name);
        }

        //

        private static int ToggleValue(Property property, string name)
        {
            int status = 2; // failed
            if (!string.IsNullOrWhiteSpace(name) && Regex.IsMatch(name, "^.+#[0-9]{4}$"))
            {
                if (!property.Value.Split(',').Contains(name)) // does not contain name, add
                {
                    List<string> list = property.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    list.Add(name);
                    property.Value = string.Join(",", list);
                    status = 0; // success, added
                }
                else // does contain mod, remove
                {
                    List<string> list = property.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (list.Count > 0)
                    {
                        int preRemoveCount = list.Count;
                        List<string> matches = list.Where(x => x.ToLower().Equals(name.ToLower())).ToList(); // Also removes duplicates, should not happen but you never know
                        foreach (string mod in matches)
                        {
                            list.Remove(mod);
                        }
                        if (list.Count != preRemoveCount)
                        {
                            property.Value = string.Join(",", list);
                        }
                    }
                    status = 1; // success, removed
                }
            }
            return status;
        }
    }
}
