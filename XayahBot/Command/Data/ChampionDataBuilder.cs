using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using XayahBot.API.Error;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Data
{
    public class ChampionDataBuilder
    {
        private readonly IMessageChannel _channel;
        private readonly string _requestedName;
        private readonly StaticDataApi _staticDataApi = new StaticDataApi(Region.EUW);
        private ChampionDto _champion;
        private DiscordFormatEmbed _message = new DiscordFormatEmbed();

        public ChampionDataBuilder(IMessageChannel channel, string requestedName)
        {
            this._channel = channel;
            this._requestedName = requestedName.Trim();
        }

        public async Task BuildAsync(ChampionDataType type)
        {
            try
            {
                if (await this.IsBuildableAsync())
                {
                    this.AppendName();
                    switch (type)
                    {
                        case ChampionDataType.MISC:
                            this.AppendMiscData();
                            break;
                        case ChampionDataType.SPELLS:
                            this.AppendSpellData();
                            break;
                        case ChampionDataType.STATS:
                            this.AppendStatisticData();
                            break;
                    }
                }
            }
            catch (ErrorResponseException)
            {
                this._message.AppendDescription("There seems to be a problem communicating with the API.");
            }
        }

        private async Task<bool> IsBuildableAsync()
        {
            bool buildable = false;
            List<ChampionDto> matches = await this.GetMatchingChampions();
            if (matches.Count == 0)
            {
                this.NoResult();
            }
            else if (matches.Count > 1)
            {
                this.MultipleResults(matches);
            }
            else
            {
                buildable = true;
                this._champion = await this._staticDataApi.GetChampionAsync(matches.First().Id);
            }
            return buildable;
        }

        private async Task<List<ChampionDto>> GetMatchingChampions()
        {
            ChampionListDto championList = await this._staticDataApi.GetChampionsAsync();
            List<ChampionDto> matches = new List<ChampionDto>();
            if (championList != null)
            {
                matches = championList.Data.Values.Where(x => x.Name.ToLower().Contains(this._requestedName.ToLower()) ||
                    StripName(x.Name).ToLower().Contains(this._requestedName.ToLower())).ToList();
            }
            matches.Sort((a, b) => a.Name.CompareTo(b.Name));
            return matches;
        }

        private string StripName(string name)
        {
            return Regex.Replace(name, @"[^ a-zA-Z0-9]+", string.Empty);
        }

        private void NoResult()
        {
            this._message.AppendDescription($"Oops! Your bad. I could not find a champion (fully or partially) named `{this._requestedName}`.");
        }

        private void MultipleResults(List<ChampionDto> matches)
        {
            this._message.AppendDescription($"Found more than one champion (fully or partially) named `{this._requestedName}`.");
            foreach (ChampionDto champion in matches)
            {
                this._message.AppendDescription(Environment.NewLine + champion.Name);
            }
        }

        private void AppendName()
        {
            this._message.AppendDescription($"Here is the requested data for `{this._champion.Name}`.");
        }

        private void AppendMiscData()
        {
            List<SkinDto> skins = this._champion.Skins.Where(x => x.Num > 0).ToList();
            skins.Sort((a, b) => a.Num.CompareTo(b.Num));
            this._champion.Tags.Sort();
            string text =
                $"Title    - {this._champion.Title}" +
                Environment.NewLine +
                $"Tags     - {string.Join(", ", this._champion.Tags)}" +
                Environment.NewLine +
                $"Resource - {this._champion.Resource}" +
                Environment.NewLine +
                "Skins     -";
            foreach (SkinDto skin in skins)
            {
                text += $"{Environment.NewLine}{skin.Num.ToString().PadLeft(2)} - {skin.Name}";
            }
            this._message.AppendDescription(text, AppendOption.Codeblock);
        }

        private void AppendSpellData()
        {
            string text = $"Passive  - {this._champion.Passive.Name}";
            foreach(ChampionSpellDto spell in this._champion.Spells)
            {
                text += $"{Environment.NewLine}------------{Environment.NewLine}" +
                    $"Name     - {spell.Name}{Environment.NewLine}" +
                    $"Cost     - {spell.GetCostString()}{Environment.NewLine}" +
                    $"Range    - {spell.GetRangeString()}{Environment.NewLine}" +
                    $"Cooldown - {spell.GetCooldownString()}{Environment.NewLine}" +
                    $"Scaling  - {spell.GetVarsString()}";
            }
            this._message.AppendDescription(text, AppendOption.Codeblock);
        }

        private void AppendStatisticData()
        {
            string text = string.Empty;
            StatsDto stats = this._champion.Stats;
            DecimalAlign statList = new DecimalAlign(stats.GetStats());
            DecimalAlign statGrowthList = new DecimalAlign(stats.GetStatGrowthList());
            text +=
                $"Health         - {statList.Align(stats.Hp)} | + {statGrowthList.TrimmedAlign(stats.HpPerLevel)}" +
                Environment.NewLine +
                $"Health Regen.  - {statList.Align(stats.HpRegen)} | + {statGrowthList.TrimmedAlign(stats.HpRegenPerLevel)}" +
                Environment.NewLine +
                $"Mana           - {statList.Align(stats.Mp)} | + {statGrowthList.TrimmedAlign(stats.MpPerLevel)}" +
                Environment.NewLine +
                $"Mana Regen     - {statList.Align(stats.MpRegen)} | + {statGrowthList.TrimmedAlign(stats.MpRegenPerLevel)}" +
                Environment.NewLine +
                $"Attack Damage  - {statList.Align(stats.AttackDamage)} | + {statGrowthList.TrimmedAlign(stats.AttackDamagePerLevel)}" +
                Environment.NewLine +
                $"Attack Speed   - {statList.Align(stats.GetBaseAttackSpeed())} | + {statGrowthList.TrimmedAlign(stats.AttackSpeedPerLevel)}%" +
                Environment.NewLine +
                $"Armor          - {statList.Align(stats.Armor)} | + {statGrowthList.TrimmedAlign(stats.ArmorPerLevel)}" +
                Environment.NewLine +
                $"Magic Resist   - {statList.Align(stats.Spellblock)} | + {statGrowthList.TrimmedAlign(stats.SpellblockPerLevel)}" +
                Environment.NewLine +
                $"Attack Range   - {statList.Align(stats.AttackRange)}" +
                Environment.NewLine +
                $"Movement Speed - {statList.Align(stats.MoveSpeed)}";
            this._message.AppendDescription(text, AppendOption.Codeblock);
        }

        public async Task PostAsync()
        {
            await this._channel.SendMessageAsync("", false, this._message.ToEmbed());
        }
    }
}
