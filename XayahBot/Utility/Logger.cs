using System;
using System.Threading.Tasks;
using Discord;

namespace XayahBot.Utility
{
    public static class Logger
    {
        public static Task Log(string source, string message)
        {
            return Log(new LogMessage(LogSeverity.Info, source, message));
        }

        public static Task Log(LogSeverity level, string source, string message)
        {
            return Log(new LogMessage(level, source, message));
        }

        public static Task Log(LogMessage message)
        {
            ConsoleColor color = Console.ForegroundColor;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{GetTimeString()} {ToSize(message.Severity, 4)} {ToSize(message.Source, 10)}: {message.Message}");
            if (message.Exception != null)
            {
                Console.WriteLine($"{GetTimeString()} {ToSize(message.Severity, 4)} {ToSize(message.Source, 10)}: {message.Exception}");
            }
            Console.ForegroundColor = color;
            return Task.CompletedTask;
        }

        //

        private static string GetTimeString()
        {
            DateTime stamp = DateTime.Now;
            return $"{ToSize(stamp.Hour, 2, '0', true)}:{ToSize(stamp.Minute, 2, '0', true)}:{ToSize(stamp.Second, 2, '0', true)}.{ToSize(stamp.Millisecond, 3, '0', true)}";
        }

        private static string ToSize(object value, int size)
        {
            return ToSize(value, size, ' ', false);
        }

        private static string ToSize(object value, int size, char padChar, bool padLeft)
        {
            string text = value != null ? value.ToString() : string.Empty;
            if (padLeft)
            {
                text = text.PadLeft(size, padChar);
            }
            else
            {
                text = text.PadRight(size, padChar);
            }
            if (text.Length > size)
            {
                text = text.Substring(0, size);
            }
            return text;
        }
    }
}
