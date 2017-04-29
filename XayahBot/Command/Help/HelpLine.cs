namespace XayahBot.Command.Help
{
    public class HelpLine
    {
        public string Command { get; set; }
        public string Summary { get; set; }

        //

        public override string ToString()
        {
            return $"{this.Command} - {this.Summary}";
        }
    }
}
