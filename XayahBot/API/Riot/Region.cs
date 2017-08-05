using System.Collections.Generic;
using System.Linq;
using XayahBot.Error;

namespace XayahBot.API.Riot
{
    public class Region
    {
        public static readonly Region BR = new Region("BR", "br1");
        public static readonly Region EUNE = new Region("EUNE", "eun1");
        public static readonly Region EUW = new Region("EUW", "euw1");
        public static readonly Region JP = new Region("JP", "jp1");
        public static readonly Region KR = new Region("KR", "kr");
        public static readonly Region LAN = new Region("LAN", "la1");
        public static readonly Region LAS = new Region("LAS", "la2");
        public static readonly Region NA = new Region("NA", "na1");
        public static readonly Region OCE = new Region("OCE", "oc1");
        public static readonly Region RU = new Region("RU", "ru");
        public static readonly Region TR = new Region("TR", "tr1");

        public static IEnumerable<Region> Values
        {
            get
            {
                yield return BR;
                yield return EUNE;
                yield return EUW;
                yield return JP;
                yield return KR;
                yield return LAN;
                yield return LAS;
                yield return NA;
                yield return OCE;
                yield return RU;
                yield return TR;
            }
        }

        public static Region GetByName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                Region match = Values.FirstOrDefault(x => x.Name.ToLower().Equals(name));
                if (match != null)
                {
                    return match;
                }
            }
            throw new NotExistingException();
        }

        // ---

        public string Name { get; private set; }
        public string Platform { get; private set; }

        private Region(string name, string platform)
        {
            this.Name = name;
            this.Platform = platform;
        }

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
