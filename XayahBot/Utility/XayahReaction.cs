using Discord;

namespace XayahBot.Utility
{
    public static class XayahReaction
    {
        public static readonly IEmote Success = new Emoji("✅");
        public static readonly IEmote Warning = new Emoji("⚠️");
        public static readonly IEmote Error = new Emoji("❌");
        public static readonly IEmote Question = new Emoji("❓");
        public static readonly IEmote Hourglass = new Emoji("⏳");
        public static readonly IEmote Clock = new Emoji("🕒");
        public static readonly IEmote Option = new Emoji("⚙️");
    }
}
