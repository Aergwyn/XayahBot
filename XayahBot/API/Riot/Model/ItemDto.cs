using System.Collections.Generic;

namespace XayahBot.API.Riot.Model
{
    public class ItemDto
    {
        //public string Colloq { get; set; }
        //public bool Consumed { get; set; }
        //public bool ConsumeOnFull { get; set; }
        //public int Depth { get; set; }
        //public string Description { get; set; }
        //public Dictionary<string, string> Effect { get; set; }
        public List<int> From { get; set; }
        public GoldDto Gold { get; set; }
        //public string Group { get; set; }
        //public bool HideFromAll { get; set; }
        public int Id { get; set; }
        //public ImageDto Image { get; set; }
        //public bool InStore { get; set; }
        public List<int> Into { get; set; }
        //public Dictionary<string, bool> Maps { get; set; }
        public string Name { get; set; }
        //public string Plaintext { get; set; }
        //public string RequiredChampion { get; set; }
        public string SanitizedDescription { get; set; }
        //public int SpecialRecipe { get; set; }
        //public int Stacks { get; set; }
        //public InventoryDataStatsDto Stats { get; set; }
        //public List<string> Tags { get; set; }
    }
}
