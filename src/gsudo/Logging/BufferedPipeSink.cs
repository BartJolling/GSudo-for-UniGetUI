using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;

namespace gsudo.Logging;

class BufferedPipeSink : ILogSink
{
    private readonly List<(string, LogLevel)> _buffer = [];
    private NamedPipeServerStream _pipeStream;
    private StreamWriter _pipeWriter;

    public void Log(string message, LogLevel level)
    {
        if (level < Settings.LogLevel) return;

        if (_pipeStream?.IsConnected == true)
        {
            _pipeWriter.WriteLine($"{Constants.TOKEN_ERROR}[Elevated]{level}: {message}{Constants.TOKEN_ERROR}");
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
            Logger.Instance.Log("BufferedPipeSink did not receive a stream to a opened named pipe", LogLevel.Warning);
            return; // let's not block
        }

        _pipeStream = controlPipe;
        _pipeWriter = new StreamWriter(controlPipe) { AutoFlush = true };

        foreach (var (msg, level) in _buffer)
        {
            _pipeWriter.WriteLine($"{Constants.TOKEN_ERROR}[Elevated]{level}: {msg}{Constants.TOKEN_ERROR}");
        }
        _buffer.Clear();
    }
}