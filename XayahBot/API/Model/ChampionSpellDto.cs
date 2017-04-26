using System.Collections.Generic;
using System.Globalization;

namespace XayahBot.API.Model
{
    public class ChampionSpellDto
    {
        //public List<ImageDto> AltImages { get; set; }
        public List<double> Cooldown { get; set; }
        public string CooldownBurn { get; set; }
        public List<int> Cost { get; set; }
        public string CostBurn { get; set; }
        //public string CostType { get; set; }
        //public string Description { get; set; }
        //public List<List<double>> Effect { get; set; } // What? Rito pls?
        //public List<string> EffectBurn { get; set; }
        //public ImageDto Image { get; set; }
        //public string Key { get; set; }
        //public LevelTipDto LevelTip { get; set; }
        public int MaxRank { get; set; }
        public string Name { get; set; }
        public object Range { get; set; } // Either List<int> or string, c'mon Rito
        public string RangeBurn { get; set; }
        //public string Resource { get; set; }
        //public string SanitizedDescription { get; set; }
        //public string SanitizedTooltip { get; set; }
        //public string Tooltip { get; set; }
        public List<SpellVarsDto> Vars { get; set; }

        //

        private string _na = "N/A";

        //

        public string GetCostString()
        {
            return this.CostBurn != "0" ? this.CostBurn : this._na;
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
                        foreach (decimal coEff in var.CoEff)
                        {
                            coeffList.Add($"{(coEff * 100).ToString("G0", CultureInfo.InvariantCulture)}%");
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
