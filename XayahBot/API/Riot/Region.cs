using System.Collections.Generic;

namespace XayahBot.API
{
    public class Region
    {
        public static readonly Region EUW = new Region("EUW", "euw1");
        public static readonly Region NA = new Region("NA", "na1");

        public static IEnumerable<Region> Values
        {
            get
            {
                yield return EUW;
                yield return NA;
            }
        }

        //

        public string Name { get; private set; }
        public string Platform { get; private set; }

        private Region(string name, string platform)
        {
            this.Name = name;
            this.Platform = platform;
        }

        //

        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj?.ToString());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
