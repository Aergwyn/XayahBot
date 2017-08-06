using Discord;

namespace XayahBot.Utility
{
    public static class XayahReaction
    {
        public static readonly IEmote Success = new Emoji("✅");
        public static readonly IEmote Warning = new Emoji("⚠️");
        public static readonly IEmote Error = new Emoji("❌");
        public static readonly IEmote Question = new Emoji("❓");
        public static readonly IEmote Time = new Emoji("⏳");
        public static readonly IEmote Option = new Emoji("⚙️");
        public static readonly IEmote LeftArrow = new Emoji("⬅");
        public static readonly IEmote RightArrow = new Emoji("➡");
    }
}
