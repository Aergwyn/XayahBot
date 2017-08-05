namespace XayahBot.Utility.Messages
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
            if (string.IsNullOrWhiteSpace(text))
            {
                this.Text += text;
            }
            else
            {
                this.Text += AppendOption.Start(options) + text + AppendOption.End(options);
            }
            return this;
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
