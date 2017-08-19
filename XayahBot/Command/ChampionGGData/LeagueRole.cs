using System.Collections.Generic;
using System.Linq;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Command.ChampionGGData
{
    public class LeagueRole
    {
        public static readonly LeagueRole All = new LeagueRole("all");
        public static readonly LeagueRole Top = new LeagueRole("Top", "TOP", "top");
        public static readonly LeagueRole Jungle = new LeagueRole("Jungle", "JUNGLE", "jungle", "jgl");
        public static readonly LeagueRole Mid = new LeagueRole("Mid", "MIDDLE", "middle", "mid");
        public static readonly LeagueRole Adc = new LeagueRole("ADC", "DUO_CARRY", "adc");
        public static readonly LeagueRole Support = new LeagueRole("Support", "DUO_SUPPORT", "support", "sup");

        public static IEnumerable<LeagueRole> Values()
        {
            yield return Top;
            yield return Jungle;
            yield return Mid;
            yield return Adc;
            yield return Support;
        }

        public static LeagueRole Get(string role)
        {
            if (!string.IsNullOrWhiteSpace(role))
            {
                role = role.ToLower();
                LeagueRole match = Values().FirstOrDefault(x => x._matches.Contains(role));
                if (match != null)
                {
                    return match;
                }
            }
            throw new NotExistingException();
        }

        public static LeagueRole GetByApiRole(string role)
        {
            if (!string.IsNullOrWhiteSpace(role))
            {
                role = role.ToUpper();
                LeagueRole match = Values().FirstOrDefault(x => x.ApiRole.Equals(role));
                if (match != null)
                {
                    return match;
                }
            }
            throw new NotExistingException();
        }

        // ---

        public string Name { get; private set; }
        public string ApiRole { get; private set; }
        private List<string> _matches = new List<string>();

        private LeagueRole(params string[] matches)
        {
            this.Name = string.Empty;
            this.ApiRole = string.Empty;
            this._matches.AddRange(matches);
        }

        private LeagueRole(string name, string apiRole, params string[] matches)
        {
            this.Name = name;
            this.ApiRole = apiRole;
            this._matches.AddRange(matches);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is LeagueRole)
            {
                LeagueRole compObj = obj as LeagueRole;
                return this.ApiRole.Equals(compObj.ApiRole);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return ListUtil.BuildEnumeration(this._matches);
        }
    }
}
