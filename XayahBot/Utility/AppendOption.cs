using System;
using System.Linq;

namespace XayahBot.Utility
{
    public class AppendOption
    {
        public static readonly AppendOption Bold = new AppendOption("**");
        public static readonly AppendOption Italic = new AppendOption("*");
        public static readonly AppendOption Strikethrough = new AppendOption("~~");
        public static readonly AppendOption Underscore = new AppendOption("__");

        public static string Start(params AppendOption[] options)
        {
            string text = string.Empty;
            foreach (AppendOption option in options)
            {
                text += option.FormatChars;
            }
            return text;
        }

        public static string End(params AppendOption[] options)
        {
            return string.Join(string.Empty, Start(options).Reverse());
        }

        //

        private string FormatChars { get; set; }

        private AppendOption(string formatChars)
        {
            this.FormatChars = formatChars;
        }

        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj?.ToString());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.FormatChars;
        }
    }
}
