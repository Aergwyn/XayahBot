using System;
using Discord;
using Discord.Commands;

namespace XayahBot.Utility.Messages
{
    public class DiscordFormatEmbed
    {
        private EmbedBuilder _builder;

        public DiscordFormatEmbed()
        {
            this._builder = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder(),
                Footer = new EmbedFooterBuilder()
            };
            this.SetColor(XayahColor.Purple);
        }

        public DiscordFormatEmbed SetColor(Color color)
        {
            this._builder.Color = color;
            return this;
        }

        public DiscordFormatEmbed SetThumbnail(string thumbnailUrl)
        {
            this._builder.ThumbnailUrl = thumbnailUrl;
            return this;
        }

        public DiscordFormatEmbed AppendTitle(string text, params AppendOption[] options)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                this._builder.Title += AppendOption.Start(options) + text + AppendOption.End(options);
            }
            return this;
        }

        public DiscordFormatEmbed AppendDescription(string text, params AppendOption[] options)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                this._builder.Description += AppendOption.Start(options) + text + AppendOption.End(options);
            }
            return this;
        }

        public DiscordFormatEmbed AppendDescriptionCodeBlock(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                this._builder.Description += Environment.NewLine + "```" + Environment.NewLine + text + Environment.NewLine + "```" + Environment.NewLine;
            }
            return this;
        }

        public DiscordFormatEmbed AddField(string name, string value, FieldFormatType? formatType = null, params AppendOption[] options)
        {
            string nameText = name;
            string valueText = value;
            switch (formatType) {
                case FieldFormatType.NAME:
                    nameText = AppendOption.Start(options) + name + AppendOption.End(options);
                    break;
                case FieldFormatType.VALUE:
                    valueText = AppendOption.Start(options) + value + AppendOption.End(options);
                    break;
                case FieldFormatType.BOTH:
                    nameText = AppendOption.Start(options) + name + AppendOption.End(options);
                    valueText = AppendOption.Start(options) + value + AppendOption.End(options);
                    break;
            }
            this._builder.AddField(nameText, valueText);
            return this;
        }

        public DiscordFormatEmbed CreateFooter(CommandContext commandContext)
        {
            this.SetFooterThumbnail(commandContext.User.GetAvatarUrl())
                .AppendFooter(commandContext.User.ToString())
                .SetFooterTimestamp();
            return this;
        }

        public DiscordFormatEmbed SetFooterThumbnail(string thumbnailUrl)
        {
            this._builder.Footer.IconUrl = thumbnailUrl;
            return this;
        }

        public DiscordFormatEmbed SetFooterTimestamp()
        {
            this._builder.Timestamp = DateTimeOffset.UtcNow;
            return this;
        }

        public DiscordFormatEmbed AppendFooter(string text)
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
    }
}
