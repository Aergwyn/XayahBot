using System;

namespace XayahBot.API.Riot.Model
{
    public class StatsDto
    {
        public decimal Armor { get; set; }
        public decimal ArmorPerLevel { get; set; }
        public decimal AttackDamage { get; set; }
        public decimal AttackDamagePerLevel { get; set; }
        public decimal AttackRange { get; set; }
        public decimal AttackSpeedOffset { get; set; }
        public decimal AttackSpeedPerLevel { get; set; }
        public decimal Crit { get; set; }
        public decimal CritPerLevel { get; set; }
        public decimal Hp { get; set; }
        public decimal HpPerLevel { get; set; }
        public decimal HpRegen { get; set; }
        public decimal HpRegenPerLevel { get; set; }
        public decimal MoveSpeed { get; set; }
        public decimal Mp { get; set; }
        public decimal MpPerLevel { get; set; }
        public decimal MpRegen { get; set; }
        public decimal MpRegenPerLevel { get; set; }
        public decimal Spellblock { get; set; }
        public decimal SpellblockPerLevel { get; set; }

        public decimal GetBaseAttackSpeed()
        {
            return Math.Round(0.625M / (1 + this.AttackSpeedOffset), 3, MidpointRounding.AwayFromZero);
        }

        public decimal[] GetStats()
        {
            return new decimal[]{
                this.Armor,
                this.AttackDamage,
                this.AttackRange,
                this.AttackSpeedOffset,
                this.Crit,
                this.Hp,
                this.HpRegen,
                this.MoveSpeed,
                this.Mp,
                this.MpRegen,
                this.Spellblock,
            };
        }

        public decimal[] GetStatGrowthList()
        {
            return new decimal[]{
                this.ArmorPerLevel,
                this.AttackDamagePerLevel,
                this.AttackSpeedPerLevel,
                this.CritPerLevel,
                this.HpPerLevel,
                this.HpRegenPerLevel,
                this.MpPerLevel,
                this.MpRegenPerLevel,
                this.SpellblockPerLevel
            };
        }
    }
}
