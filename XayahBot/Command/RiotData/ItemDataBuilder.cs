using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.RiotData
{
    public class ItemDataBuilder
    {
        public static async Task<FormattedEmbedBuilder> BuildAsync(string name)
        {
            name = name.Trim();
            ItemDataBuilder itemBuilder = new ItemDataBuilder();
            FormattedEmbedBuilder message = new FormattedEmbedBuilder();
            try
            {
                List<ItemDto> matches = await itemBuilder.GetMatchingItemsAsync(name);
                if (matches.Count == 0)
                {
                    message
                        .AppendTitle($"{XayahReaction.Error} This didn't work")
                        .AppendDescription($"Oops! Your bad. I couldn't find an item (fully or partially) named `{name}`.");
                }
                else if (matches.Count > 1)
                {
                    message
                        .AppendTitle($"{XayahReaction.Warning} This didn't went as expected")
                        .AppendDescription($"I found more than one item (fully or partially) named `{name}`.");
                    foreach (ItemDto item in matches)
                    {
                        message.AppendDescription(Environment.NewLine + item.Name);
                    }
                }
                else
                {
                    ItemDto item = matches.First();
                    await itemBuilder.AppendData(item, message);
                }
            }
            catch (NoApiResultException)
            {
                message = new FormattedEmbedBuilder()
                    .AppendTitle($"{XayahReaction.Error} This didn't work")
                    .AppendDescription("Apparently some random API refuses cooperation. Have some patience while I convince them again...");
            }
            return message;
        }

        // ---

        private readonly RiotStaticData _riotStaticData = new RiotStaticData(Region.EUW);

        private ItemDataBuilder()
        {
        }

        private async Task<List<ItemDto>> GetMatchingItemsAsync(string name)
        {
            name = name.ToLower();
            ItemListDto itemList = await this._riotStaticData.GetItemsAsync();
            List<ItemDto> matches = new List<ItemDto>();
            if (itemList != null)
            {
                matches = itemList.Data.Values
                    .Where(x => x.Name.ToLower().Contains(name) || StripName(x.Name).ToLower().Contains(name)).ToList();
            }
            matches.Sort((a, b) => a.Name.CompareTo(b.Name));
            return matches;
        }

        private string StripName(string name)
        {
            return Regex.Replace(name, @"[^ a-zA-Z0-9]+", string.Empty);
        }

        private async Task AppendData(ItemDto item, FormattedEmbedBuilder message)
        {
            message
               .SetThumbnail($"http://ddragon.leagueoflegends.com/cdn/{Property.RiotUrlVersion.Value}/img/item/{item.Id}.png")
               .AppendTitle(item.Name, AppendOption.Bold, AppendOption.Underscore)
               .AppendDescription(item.SanitizedDescription)
               .AppendDescriptionNewLine(2)
               .AppendDescription($"Total Cost:", AppendOption.Italic).AppendDescription($" {item.Gold.Total} Gold")
               .AppendDescriptionNewLine()
               .AppendDescription($"Selling for:", AppendOption.Italic).AppendDescription($" {item.Gold.Sell} Gold")
               .AppendDescriptionNewLine()
               .AppendDescription($"Completion Cost:", AppendOption.Italic).AppendDescription($" {item.Gold.Base} Gold");
            if (item.From.Count > 0)
            {
                message.AppendDescriptionNewLine(2)
                    .AppendDescription("Components", AppendOption.Bold, AppendOption.Underscore);

                List<ItemDto> componentList = await this.GetFromList(item.From);
                componentList.Sort((a, b) => a.Gold.Base.CompareTo(b.Gold.Base));
                foreach (ItemDto component in componentList)
                {
                    message.AppendDescriptionNewLine()
                     .AppendDescription($"{component.Name} ({component.Gold.Base} Gold)");
                }
            }
            if (item.Into.Count > 0)
            {
                message.AppendDescriptionNewLine(2)
                    .AppendDescription("Builds Into", AppendOption.Bold, AppendOption.Underscore);

                List<ItemDto> parentList = await this.GetFromList(item.Into);
                parentList.Sort((a, b) => a.Gold.Base.CompareTo(b.Gold.Base));
                foreach (ItemDto parent in parentList)
                {
                    message.AppendDescriptionNewLine()
                        .AppendDescription($"{parent.Name} ({parent.Gold.Base} Gold)");
                }
            }
        }

        private async Task<List<ItemDto>> GetFromList(List<int> ids)
        {
            List<ItemDto> items = new List<ItemDto>();
            if (ids.Count > 0)
            {
                ItemListDto itemList = await this._riotStaticData.GetItemsAsync();
                foreach (int id in ids)
                {
                    items.Add(itemList.Data.Values.FirstOrDefault(x => x.Id.Equals(id)));
                }
            }
            return items;
        }
    }
}
