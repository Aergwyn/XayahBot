using System;
using System.Collections.Generic;
using System.Linq;

namespace XayahBot.Utility
{
    public static class RNG
    {
        public static int Next(int max = 100)
        {
            return new Random().Next(1, max + 1);
        }

        public static int Next(int min, int max)
        {
            return new Random().Next(min, max + 1);
        }

        public static T FromList<T>(List<T> list)
        {
            return list.ElementAt(Next(list.Count) - 1);
        }
    }
}
