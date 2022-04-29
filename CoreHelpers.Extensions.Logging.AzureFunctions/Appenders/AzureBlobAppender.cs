using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using CoreHelpers.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Appenders
{
    internal class AzureBlobAppender : ILogAppender
    {
        private string _connectionString;
        private string _containerName;
        private string _logBlobName;

        private BlobServiceClient _blobServiceClient;
        private BlobContainerClient _blobContainerClient;
        private AppendBlobClient _appendBlobClient;

        private MemoryStream _logBuffer = new MemoryStream();
        private int _bufferedMessages;
        private int _processedLines;

        public AzureBlobAppender(string connectionString, string containerName, string logBlobName, int bufferedMessages)
        {
            _connectionString = connectionString;
            _containerName = containerName;
            _logBlobName = logBlobName;
            _bufferedMessages = bufferedMessages;
            _processedLines = 0;
        }

        public void Append<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {            
            var message = $"[{DateTime.Now.ToString("o")}] {formatter(state, exception)}{Environment.NewLine}";
            this.AppendLogLine(message, false);
        }
               
        public void Dispose()
        {
            Flush();
        }

        public void Open()
        {
            // build the conenction string
            var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2021_02_12);
            _blobServiceClient = new BlobServiceClient(_connectionString, options);

            // verify the container exists if not try to create
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            if (!_blobContainerClient.Exists())
                _blobContainerClient.Create(Azure.Storage.Blobs.Models.PublicAccessType.None);

            _appendBlobClient = _blobContainerClient.GetAppendBlobClient(_logBlobName);
        }

        private void AppendLogLine(string logLine, bool flushInstantly = false)
        {
            if (string.IsNullOrEmpty(logLine))
                return;

            lock (_logBuffer)
            {
                // write the log line to the buffer
                var logLineBytes = Encoding.Unicode.GetBytes(logLine);
                _logBuffer.Write(logLineBytes, 0, logLineBytes.Length);
                _processedLines++;

                // flush the buffer if needed
                if (_processedLines >= _bufferedMessages || flushInstantly)
                    Flush();
            }
        }

        public void Flush()
        {
            Flush(false);
        }

        private void Flush(bool secondCall = false)
        {
            lock (_logBuffer)
            {
                try
                {
                    if (_logBuffer.Length == 0)
                        return;

                    // add the data 
                    _logBuffer.Position = 0;
                    _appendBlobClient.AppendBlock(_logBuffer);

                    // reset the buffer
                    _logBuffer.SetLength(0);
                    _logBuffer.Position = 0;
                }
                catch (Azure.RequestFailedException ex)
                {
                    if (!secondCall && ex.ErrorCode == "BlobNotFound")
                    {
                        // create the blob
                        _appendBlobClient.Create();

                        // Flush again
                        Flush(true);

                    }
                    else
                    {
                        ExceptionDispatchInfo.Capture(ex).Throw();
                        return;
                    }
                }
            }
        }
    }
}
