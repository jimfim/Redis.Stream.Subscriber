using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Stream.Subscriber
{
    public class RedisRedisStreamClient : IRedisStreamClient
    {
        private readonly NetworkStream _streamClient;


        public RedisRedisStreamClient(NetworkStream streamClient)
        {
            _streamClient = streamClient;
        }
        
        public async Task ReadStreamEventsForwardAsync(string streamName, 
            long lastCheckpoint,
            SubscriptionSettings settings,
            Func<ResolvedEvent, Task> eventAppeared,
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = ClientCommands.Subscribe(streamName, settings.BatchSize, lastCheckpoint);
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

                var parsedStreamData = streamDataBuffer.ToString().Split("\r\n");
                
                try
                {
                    await eventAppeared.Invoke(new ResolvedEvent
                    {
                        Id = parsedStreamData[7],
                        Stream = parsedStreamData[3],
                        FieldName = parsedStreamData[10],
                        Data = parsedStreamData[12]
                    });
                    lastCheckpoint += 1;
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }
            }
        }

        public async Task ReadStreamEventsForwardAsync(string streamName, long lastCheckpoint, Func<ResolvedEvent, Task> eventAppeared,
            CancellationToken cancellationToken)
        {
            await ReadStreamEventsForwardAsync(streamName, lastCheckpoint, new SubscriptionSettings(), eventAppeared, cancellationToken);
        }

        public void Close()
        {
            _streamClient.Close();
            _streamClient.Dispose();
        }
    }
}