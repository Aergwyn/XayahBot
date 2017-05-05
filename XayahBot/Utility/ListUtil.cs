using System.Collections.Generic;
using System.Linq;

namespace XayahBot.Utility
{
    public static class ListUtil
    {
        public static string BuildEnumeration<T>(IEnumerable<T> list)
        {
            string text = string.Empty;
            for (int i = 0; i < list.Count(); i++)
            {
                if (i > 0)
                {
                    text += ", ";
                }
                text += list.ElementAt(i).ToString();
            }
            return text;
        }

        public static string BuildAndEnumeration<T>(IEnumerable<T> list)
        {
            return BuildCustomEnumeration(list, "and");
        }

        public static string BuildOrEnumeration<T>(IEnumerable<T> list)
        {
            return BuildCustomEnumeration(list, "or");
        }

        private static string BuildCustomEnumeration<T>(IEnumerable<T> list, string word)
        {
            string text = string.Empty;
            for (int i = 0; i < list.Count(); i++)
            {
                if (i > 0)
                {
                    if (i == list.Count() - 1)
                    {
                        text += $" {word} ";
                    }
                    else
                    {
                        text += ", ";
                    }
                }
                text += $"`{list.ElementAt(i).ToString()}`";
            }
            return text;
        }
    }
}
