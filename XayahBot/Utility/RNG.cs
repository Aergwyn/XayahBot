using System;
using System.Collections.Generic;
using System.Linq;

namespace XayahBot.Utility
{
    public class RNG
    {
        private Random _numberGen = new Random();

        //

        public int Next(int max = 100)
        {
            return this._numberGen.Next(1, max + 1);
        }

        public int Next(int min, int max)
        {
            return this._numberGen.Next(min, max + 1);
        }

        //

        public T FromList<T>(List<T> list)
        {
            return list.ElementAt(this.Next(list.Count) - 1);
        }
    }
}
