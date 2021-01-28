using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Redis.Stream.Subscriber
{
    public class RedisRedisStreamClient : IRedisStreamClient
    {
        private readonly NetworkStream _streamClient;


        public RedisRedisStreamClient(NetworkStream streamClient)
        {
            _streamClient = streamClient;
        }
        
        
        public async IAsyncEnumerable<StreamEntry> ReadStreamAsync(string streamName, 
            uint lastCheckpoint,
            SubscriptionSettings settings,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var index = lastCheckpoint;
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = CommandConstants.Subscribe(streamName, settings.BatchSize, index);
                var bytes = Encoding.ASCII.GetBytes(message);
                if (_streamClient.CanWrite)
                {
                    await _streamClient.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                }
                else
                {
                    await Console.Out.WriteLineAsync("Can't write");
                    continue;
                }

                var streamDataBuffer = new StringBuilder();
                do
                {
                    var buffer = new byte[settings.BufferSize];
                    await _streamClient.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    streamDataBuffer.Append(Encoding.ASCII.GetString(buffer, 0, buffer.Length));
                } while (_streamClient.DataAvailable);

                foreach (var streamEntry in StreamParser.Parse(streamDataBuffer))
                {
                    yield return streamEntry;
                    index++;
                }
            }
        }
        
        public async IAsyncEnumerable<StreamEntry> ReadStreamAsync(string streamName, uint lastCheckpoint, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var streamEntry in ReadStreamAsync(streamName, lastCheckpoint, new SubscriptionSettings(), cancellationToken))
            {
                yield return streamEntry;
            }
        }

        public void Close()
        {
            _streamClient.Close();
            _streamClient.Dispose();
        }
    }

    public class StreamEntry
    {
        public string FieldName { get; set; }
        public string Id { get; set; }
        public string Data { get; set; }
    }
}