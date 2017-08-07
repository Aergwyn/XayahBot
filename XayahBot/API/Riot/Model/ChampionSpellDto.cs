using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using XayahBot.Utility;

namespace XayahBot.API.Riot.Model
{
    public class ChampionSpellDto
    {
        //public List<ImageDto> AltImages { get; set; }
        public List<double> Cooldown { get; set; }
        public string CooldownBurn { get; set; }
        public List<int> Cost { get; set; }
        public string CostBurn { get; set; }
        public string CostType { get; set; }
        //public string Description { get; set; }
        //public List<List<double>> Effect { get; set; } // What? Rito pls?
        public List<string> EffectBurn { get; set; }
        //public ImageDto Image { get; set; }
        //public string Key { get; set; }
        //public LevelTipDto LevelTip { get; set; }
        public int MaxRank { get; set; }
        public string Name { get; set; }
        public object Range { get; set; } // Either List<int> or string, c'mon Rito
        public string RangeBurn { get; set; }
        public string Resource { get; set; }
        //public string SanitizedDescription { get; set; }
        //public string SanitizedTooltip { get; set; }
        //public string Tooltip { get; set; }
        public List<SpellVarsDto> Vars { get; set; }

        private string _na = "N/A";

        public string GetCostString()
        {
            string result = this._na;
            if (this.CostBurn != "0")
            {
                result = this.CostBurn + this.CostType;
            }
            else
            {
                int effectIndex = this.GetEffectIndex();
                if (effectIndex >= 0 && effectIndex < this.EffectBurn.Count)
                {
                    result = this.EffectBurn.ElementAt(effectIndex) + this.CostType;
                }
            }
            return result;
        }

        private int GetEffectIndex()
        {
            Regex regex = new Regex("{{ e[0-9] }}");
            if (regex.IsMatch(this.Resource))
            {
                Match match = regex.Match(this.Resource);
                return NumberUtil.StripForNumber(match.Value as string);
            }
            return -1;
        }

        public string GetRangeString()
        {
            return this.RangeBurn != "0" ? this.RangeBurn : this._na;
        }

        public string GetCooldownString()
        {
            return this.CooldownBurn != "0" ? this.CooldownBurn : this._na;
        }

        public string GetVarsString()
        {
            List<string> varList = new List<string>();
            List<string> coeffList = new List<string>();
            if (this.Vars != null)
            {
                foreach (SpellVarsDto var in this.Vars)
                {
                    if (var.CoEff != null)
                    {
                        foreach (decimal scaling in var.CoEff)
                        {
                            coeffList.Add($"{(scaling * 100).ToString("G0", CultureInfo.InvariantCulture)}%");
                        }
                        varList.Add(string.Join(",", coeffList));
                        coeffList.Clear();
                    }
                }
            }
            string result = this._na;
            if (varList.Count > 0)
            {
                result = string.Join(" | ", varList);
            }
            return result;
        }
    }
}
