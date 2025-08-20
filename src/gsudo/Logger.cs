using System.Collections.Generic;
using System.Linq;
using gsudo.Logging;

#nullable enable

namespace gsudo
{

    internal enum LogLevel
    {
        All = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        None = 5,
    }

    class Logger
    {
        public static readonly Logger Instance = new();

        private readonly List<ILogSink> _sinks = [];

        private Logger()
        {
            _sinks.Add(new ConsoleSink()); // Default sink
        }

        public void RegisterSink(ILogSink sink)
        {
            _sinks.Add(sink);
        }

        public T? GetSink<T>() where T : class, ILogSink
        {
            return _sinks.OfType<T>().FirstOrDefault();
        }

        public void Log(string message, LogLevel level)
        {
            foreach (var sink in _sinks)
            {
                sink.Log(message, level);
            }
        }
    }
}

#nullable disable