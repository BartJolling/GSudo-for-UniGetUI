using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;

namespace gsudo.Logging;

internal class BufferedPipeSink(bool SingleUse) : ILogSink
{
    private readonly List<(string, LogLevel)> _buffer = [];
    private NamedPipeServerStream _pipeStream;
    private StreamWriter _pipeWriter;
    private readonly string _marker = SingleUse ? "Elevated" : "Service";

    public void Log(string message, LogLevel level)
    {
        if (level < Settings.LogLevel) return;

        if (_pipeStream?.IsConnected == true)
        {
            WriteToPipe(message, level);
        }
        else
        {
            _buffer.Add((message, level));
        }
    }

    public void AttachPipe(NamedPipeServerStream controlPipe)
    {
        if (controlPipe is null || !controlPipe.IsConnected)
        {
            Logger.Instance.Log("BufferedPipeSink did not receive a stream to an opened named pipe", LogLevel.Warning);
            return; // don't block
        }

        _pipeStream = controlPipe;
        _pipeWriter = new StreamWriter(controlPipe) { AutoFlush = true };

        foreach (var (msg, level) in _buffer)
        {
            WriteToPipe(msg, level);
        }
        _buffer.Clear();
    }

    private void WriteToPipe(string message, LogLevel level)
    {
        if (_pipeWriter == null) return;

        string token = level >= LogLevel.Error
            ? Constants.TOKEN_ERROR
            : Constants.TOKEN_LOG;

        _pipeWriter.WriteLine($"{token}[{_marker}] {level}: {message}{token}");
    }
}