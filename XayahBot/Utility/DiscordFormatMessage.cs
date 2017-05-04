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

        public DiscordFormatMessage Append(string text, params AppendOption[] options)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                this.Text += AppendOption.Start(options) + text + AppendOption.End(options);
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

        public override string ToString()
        {
            return this.Text;
        }
    }
}
