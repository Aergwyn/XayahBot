using System;
using Discord;
using Discord.Commands;

namespace XayahBot.Utility.Messages
{
    public class FormattedEmbedBuilder
    {
        private EmbedBuilder _builder;

        public FormattedEmbedBuilder()
        {
            this._builder = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder(),
                Footer = new EmbedFooterBuilder()
            };
            this.SetColor(XayahColor.Purple);
        }

        public FormattedEmbedBuilder SetColor(Color color)
        {
            this._builder.Color = color;
            return this;
        }

        public FormattedEmbedBuilder SetThumbnail(string thumbnailUrl)
        {
            this._builder.ThumbnailUrl = thumbnailUrl;
            return this;
        }

        public FormattedEmbedBuilder AppendTitle(string text, params AppendOption[] options)
        {
            this._builder.Title += this.ApplyOptions(text, options);
            return this;
        }

        public FormattedEmbedBuilder AppendDescription(string text, params AppendOption[] options)
        {
            this._builder.Description += this.ApplyOptions(text, options);
            return this;
        }

        public FormattedEmbedBuilder AppendDescriptionNewLine(int repeat = 1)
        {
            if (repeat < 1)
            {
                repeat = 1;
            }
            for (int i = 1; i <= repeat; i++)
            {
                this._builder.Description += Environment.NewLine;
            }
            return this;
        }

        public FormattedEmbedBuilder AddField(string name, string value, AppendOption[] nameOptions = null, AppendOption[] valueOptions = null, bool inline = true)
        {
            string nameText = this.ApplyOptions(name, nameOptions);
            string valueText = this.ApplyOptions(value, valueOptions);
            this._builder.AddField(nameText, valueText, inline);
            return this;
        }

        public FormattedEmbedBuilder CreateFooterIfNotDM(ICommandContext commandContext)
        {
            if (!(commandContext as CommandContext).IsPrivate)
            {
                this.SetFooterThumbnail(commandContext.User.GetAvatarUrl())
                    .AppendFooter(commandContext.User.ToString())
                    .SetFooterTimestamp();
            }
            return this;
        }

        public FormattedEmbedBuilder SetFooterThumbnail(string thumbnailUrl)
        {
            this._builder.Footer.IconUrl = thumbnailUrl;
            return this;
        }

        public FormattedEmbedBuilder SetFooterTimestamp()
        {
            this._builder.Timestamp = DateTimeOffset.UtcNow;
            return this;
        }

        public FormattedEmbedBuilder AppendFooter(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                this._builder.Footer.Text += text;
            }
            return this;
        }

        public Embed ToEmbed()
        {
            return this._builder.Build();
        }

        private string ApplyOptions(string text, params AppendOption[] options)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            else
            {
                return AppendOption.Start(options) + text + AppendOption.End(options);
            }
        }
    }
}
