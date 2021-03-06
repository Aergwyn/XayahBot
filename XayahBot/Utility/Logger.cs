﻿using System;
using System.Threading.Tasks;
using Discord;

namespace XayahBot.Utility
{
    public static class Logger
    {
        public static void Debug(object message, Exception exception = null)
        {
            Log(LogSeverity.Debug, message?.ToString(), exception);
        }

        public static void Info(object message, Exception exception = null)
        {
            Log(LogSeverity.Info, message?.ToString(), exception);
        }

        public static void Warning(object message, Exception exception = null)
        {
            Log(LogSeverity.Warning, message?.ToString(), exception);
        }

        public static void Error(object message, Exception exception = null)
        {
            Log(LogSeverity.Warning, message?.ToString(), exception);
        }

        public static void Log(LogSeverity severity, string message, Exception exception = null)
        {
            Task.Run(() => Log(new LogMessage(severity, nameof(Logger), message, exception)).ConfigureAwait(false));
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
            Console.WriteLine($"{GetTimeString()} {PadText(message.Severity, 4)}: {message.Message}");
            if (message.Exception != null)
            {
                Console.WriteLine($"{GetTimeString()} {PadText(message.Severity, 4)}: {message.Exception}");
            }
            Console.ForegroundColor = color;
            return Task.CompletedTask;
        }

        private static string GetTimeString()
        {
            DateTime stamp = DateTime.Now;
            return $"{PadNumber(stamp.Day, 2)}.{PadNumber(stamp.Month, 2)}. " +
                $"{PadNumber(stamp.Hour, 2)}:{PadNumber(stamp.Minute, 2)}:{PadNumber(stamp.Second, 2)}.{PadNumber(stamp.Millisecond, 3)}";
        }

        private static string PadText(object text, int size)
        {
            string result = text?.ToString() ?? string.Empty;
            result = result.PadRight(size);
            return Trim(result, size);
        }

        private static string PadNumber(object number, int size)
        {
            string result = number?.ToString() ?? string.Empty;
            result = result.PadLeft(size, '0');
            return Trim(result, size);
        }

        private static string Trim(string text, int size)
        {
            if (text.Length > size)
            {
                text = text.Substring(0, size);
            }
            return text;
        }
    }
}
