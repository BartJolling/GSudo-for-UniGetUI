using System;

namespace gsudo.Logging;

internal class ConsoleSink : ILogSink
{
    public void Log(string message, LogLevel level)
    {
        if (level < Settings.LogLevel.Value) return;

        try
        {
            Console.ForegroundColor = GetColor(level);
            Console.Error.WriteLine($"{level}: {message}");
            Console.ResetColor();
        }
        catch { }
    }

    private static ConsoleColor GetColor(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => ConsoleColor.DarkGray,
            LogLevel.Info => ConsoleColor.Gray,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => ConsoleColor.Gray
        };
    }
}