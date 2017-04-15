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

        public string GetVarsString()
        {
            List<string> varList = new List<string>();
            List<string> coEffList = new List<string>();
            if (this.Vars != null)
            {
                foreach (SpellVarsDto var in this.Vars)
                {
                    if (var.CoEff != null)
                    {
                        foreach (decimal coEff in var.CoEff)
                        {
                            coEffList.Add(coEff.ToString(CultureInfo.InvariantCulture));
                        }
                        varList.Add(string.Join(",", coEffList));
                        coEffList.Clear();
                    }
                }
            }
            return string.Join("/", varList);
        }
    }
}
