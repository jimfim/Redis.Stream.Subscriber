using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Stream.Subscriber
{
    public class RedisRedisStreamClient : IRedisStreamClient
    {
        private TcpClient client;
        
        public async Task Subscribe(RedisStreamSettings settings, Func<ResolvedEvent, Task> eventAppeared,
            CancellationToken cancellationToken)
        {
            client = new TcpClient(settings.host, settings.Port);
            var stream = client.GetStream();
            
            var startingIndex = settings.StartingIndex;
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = ClientCommands.Subscribe(settings.Stream, settings.BatchSize, startingIndex);
                var bytes = Encoding.ASCII.GetBytes(message);
                if (stream.CanWrite)
                {
                    await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
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
                    await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    streamDataBuffer.Append(Encoding.ASCII.GetString(buffer, 0, buffer.Length));
                } while (stream.DataAvailable);

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
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }

                startingIndex += (uint)settings.BatchSize;
            }
        }

        public void Close()
        {
            client.Close();
            client.Dispose();
        }
    }
}