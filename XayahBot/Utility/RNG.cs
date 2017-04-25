using System;
using System.Collections.Generic;
using System.Linq;

namespace XayahBot.Utility
{
    public static class RNG
    {
        private static Random _numberGen = new Random();

        //

        public static int Next()
        {
            return _numberGen.Next(1, 101);
        } // Returns 1 to 100

        public static int Next(int max)
        {
            return _numberGen.Next(1, max + 1);
        } // Returns 1 to max

        public static int Next(int min, int max)
        {
            return _numberGen.Next(min, max + 1);
        } // Returns min to max

        //

        public static T FromList<T>(List<T> list)
        {
            return list.ElementAt(Next(list.Count) - 1);
        }
    }
}
