using System;
namespace CoreHelpers.Extensions.Logging.Abstractions
{
    public interface ILogAppenderFactory
    {
        ILogAppender CreateLogAppender(string logSinkIdentifier = null);                    
    }
}
