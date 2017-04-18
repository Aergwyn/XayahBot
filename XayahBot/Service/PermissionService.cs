using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Discord.Commands;
using XayahBot.Utility;
using System;
using Discord;

namespace XayahBot.Service
{
    public static class PermissionService
    {
        public static bool IsAdmin(IUser user)
        {
            return Property.Author.Equals(user.ToString());
        }

        public static bool IsMod(IUser user)
        {
            if (Property.CfgMods.Value.Split(',').FirstOrDefault(x => x.Equals(user.ToString())) != null)
            {
                return true;
            }
            return false;
        }

        public static bool IsAdminOrMod(IUser user)
        {
            if (IsAdmin(user) || IsMod(user))
            {
                return true;
            }
            return false;
        }

        public static bool IsIgnored(IUser user)
        {
            if (Property.CfgIgnore.Value.Split(',').FirstOrDefault(x => x.Equals(user.ToString())) != null)
            {
                return true;
            }
            return false;
        }

        //

        public static bool AddMod(string name)
        {
            return Add(Property.CfgMods, name);
        }

        public static bool RemoveMod(string name)
        {
            return Remove(Property.CfgMods, name);
        }

        public static bool AddIgnore(string name)
        {
            return Add(Property.CfgIgnore, name);
        }

        public static bool RemoveIgnore(string name)
        {
            return Remove(Property.CfgIgnore, name);
        }

        //

        private static bool Add(Property property, string name)
        {
            if (!property.Value.Split(',').Contains(name)) // does not contain name, add
            {
                List<string> list = property.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                list.Add(name);
                property.Value = string.Join(",", list);
                return true;
            }
            return false;
        }

        private static bool Remove(Property property, string name)
        {
            if (property.Value.Split(',').Contains(name)) // does not contain name, add
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
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
