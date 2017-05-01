using System;
using System.Linq;

namespace XayahBot.Utility
{
    public class DiscordFormatMessage
    {
        private string Text { get; set; }

        public DiscordFormatMessage(string text = "")
        {
            this.Text = text;
        }

        //

        public DiscordFormatMessage Append(string text, params AppendOption[] options)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                string openOptions = string.Empty;
                string closeOptions = string.Empty;
                foreach (AppendOption option in options)
                {
                    openOptions += GetOptionChars(option);
                }
                closeOptions = string.Join(string.Empty, openOptions.Reverse());
                this.Text += openOptions + text + closeOptions;
            }
            return this;
        }

        public DiscordFormatMessage AppendCodeBlock(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                this.Text += Environment.NewLine + "```" + Environment.NewLine + text + Environment.NewLine + "```" + Environment.NewLine;
            }
            return this;
        }

        private string GetOptionChars(AppendOption option)
        {
            switch (option)
            {
                case AppendOption.BOLD:
                    return "**";
                case AppendOption.ITALIC:
                    return "*";
                case AppendOption.STRIKETHROUGH:
                    return "~~";
                case AppendOption.UNDERSCORE:
                    return "__";
                default:
                    return string.Empty;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
