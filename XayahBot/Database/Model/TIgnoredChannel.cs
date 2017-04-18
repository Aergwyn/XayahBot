namespace XayahBot.Database.Model
{
    public class TIgnoredChannel
    {
        public int Id { get; set; }
        public ulong Guild { get; set; }
        public ulong Channel { get; set; }
    }
}
