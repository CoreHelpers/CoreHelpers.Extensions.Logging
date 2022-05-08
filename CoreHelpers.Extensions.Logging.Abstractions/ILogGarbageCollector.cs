using System;
using System.Threading.Tasks;

namespace CoreHelpers.Extensions.Logging.Abstractions
{
    public interface ILogGarbageCollector
    {
        Task CleanLogRepository(string connectionString, string containerName, int retentionWindowsInMonths);
    }
}
