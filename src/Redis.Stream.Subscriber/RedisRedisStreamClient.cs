using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Redis.Stream.Subscriber
{
    public class RedisRedisStreamClient : IRedisStreamClient
    {
        private readonly NetworkStream _streamClient;
        private readonly ILogger<RedisRedisStreamClient>? _logger;


        /// <summary>
        /// Creates a new instance with optional logging.
        /// </summary>
        public RedisRedisStreamClient(NetworkStream streamClient, ILogger<RedisRedisStreamClient>? logger = null)
        {
            _streamClient = streamClient ?? throw new ArgumentNullException(nameof(streamClient));
            _logger = logger;
        }


        public async IAsyncEnumerable<StreamEntry> ReadStreamAsync(string streamName,
            uint lastCheckpoint,
            SubscriptionSettings settings,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(streamName))
            {
                throw new ArgumentException("Stream name cannot be null or empty", nameof(streamName));
            }

            settings.Validate();
            
            var index = lastCheckpoint;
            while (!cancellationToken.IsCancellationRequested)
            {
                StringBuilder? streamDataBuffer = null;
                
                try
                {
                    var message = CommandConstants.Subscribe(streamName, settings.BatchSize, index);
                    var bytes = Encoding.ASCII.GetBytes(message);
                    
                    _logger?.LogDebug("Sending XREAD command");

                    if (_streamClient.CanWrite)
                    {
                        await _streamClient.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                    }
                    else
                    {
                        _logger?.LogWarning("Stream not writable, retrying...");
                        continue;
                    }

                    streamDataBuffer = new StringBuilder();
                    do
                    {
                        var buffer = new byte[settings.BufferSize];
                        await _streamClient.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        streamDataBuffer.Append(Encoding.ASCII.GetString(buffer, 0, buffer.Length));
                    } while (_streamClient.DataAvailable);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    _logger?.LogDebug("Stream reading cancelled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error reading from stream: {ErrorMessage}", ex.Message);
                    throw;
                }

                if (streamDataBuffer != null)
                {
                    await foreach (var streamEntry in ProcessStreamEntriesAsync(streamDataBuffer, cancellationToken))
                    {
                        yield return streamEntry;
                        index++;
                    }
                }
            }
        }

        private async IAsyncEnumerable<StreamEntry> ProcessStreamEntriesAsync(StringBuilder streamDataBuffer, CancellationToken cancellationToken)
        {
            foreach (var streamEntry in StreamParser.Parse(streamDataBuffer))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
                yield return streamEntry;
            }
        }

        public async IAsyncEnumerable<StreamEntry> ReadStreamAsync(string streamName, uint lastCheckpoint, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var streamEntry in ReadStreamAsync(streamName, lastCheckpoint, new SubscriptionSettings(), cancellationToken))
            {
                yield return streamEntry;
            }
        }

        public async IAsyncEnumerable<StreamEntry> ReadStreamBackwardsAsync(string streamName, string fromId = "-", int batchSize = 100, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamName))
            {
                throw new ArgumentException("Stream name cannot be null or empty", nameof(streamName));
            }

            var message = CommandConstants.ReadRangeBackwards(streamName, batchSize, fromId);
            var bytes = Encoding.ASCII.GetBytes(message);

            _logger?.LogDebug("Sending XREVRANGE command");

            if (_streamClient.CanWrite)
            {
                await _streamClient.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
            }

            var streamDataBuffer = new StringBuilder();
            while (_streamClient.DataAvailable && !cancellationToken.IsCancellationRequested)
            {
                var buffer = new byte[8192];
                var bytesRead = await _streamClient.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesRead > 0)
                {
                    streamDataBuffer.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                }
            }

            foreach (var streamEntry in StreamParser.Parse(streamDataBuffer))
            {
                yield return streamEntry;
            }
        }

        public void Close()
        {
            _streamClient.Close();
            _streamClient.Dispose();
            _logger?.LogDebug("Stream client closed");
        }
    }

    /// <summary>
    /// Represents a single entry in a Redis stream.
    /// </summary>
    public class StreamEntry
    {
        /// <summary>
        /// The unique ID of this entry.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// The field name associated with the data.
        /// </summary>
        public string FieldName { get; set; } = null!;

        /// <summary>
        /// The actual data payload of this entry.
        /// </summary>
        public string Data { get; set; } = null!;
    }
}