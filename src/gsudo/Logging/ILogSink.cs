namespace gsudo.Logging;

internal interface ILogSink
{
    void Log(string message, LogLevel level);
}