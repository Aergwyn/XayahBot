namespace XayahBot.Utility
{
    public class HelpLine
    {
        public string Command { get; set; }
        public string Summary { get; set; }

        //

        #region Overrides

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Command} - {this.Summary}";
        }

        #endregion
    }
}
