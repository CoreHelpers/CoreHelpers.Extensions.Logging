using System;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.Abstractions
{
    public interface ILogAppender : IDisposable
    {
        void Open();

        void Append<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);

        void Flush();
    }
}
