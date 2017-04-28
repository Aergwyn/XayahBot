using System.Collections.Generic;

namespace XayahBot.API
{
    public class Region
    {
        public static readonly Region EUW = new Region("EUW", "euw1");
        public static readonly Region NA = new Region("NA", "na1");

        //

        public IEnumerable<Region> Values
        {
            get
            {
                yield return EUW;
                yield return NA;
            }
        }

        //

        public string Name { get; private set; }
        public string ApiName { get; private set; }

        //

        private Region(string name, string apiName)
        {
            this.Name = name;
            this.ApiName = apiName;
        }

        //

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                string comp = obj.ToString();
                if (this.ToString().Equals(comp))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}
