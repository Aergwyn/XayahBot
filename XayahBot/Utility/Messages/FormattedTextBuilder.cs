using System;

namespace XayahBot.Utility.Messages
{
    public class FormattedTextBuilder
    {
        private string Text { get; set; }

        public FormattedTextBuilder(string text = "")
        {
            this.Text = text;
        }

        public FormattedTextBuilder Append(string text, params AppendOption[] options)
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

        public FormattedTextBuilder AppendNewLine(int repeat = 1)
        {
            if (repeat < 1)
            {
                repeat = 1;
            }
            for (int i = 1; i <= repeat; i++)
            {
                this.Text += Environment.NewLine;
            }
            return this;
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
