using System;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.Abstractions
{
    public interface IScopedLogger : ILogger
    {
        void DisposeScope<TState>(TState state);
    }
}
