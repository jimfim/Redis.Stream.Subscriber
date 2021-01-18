using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace redis_tcp
{
    public interface IRedisStreamClient
    {
        void Subscribe(RedisStreamSettings settings,
            Func<ResolvedEvent, Task> eventAppeared,
            CancellationToken cancellationToken);

    }

    public class RedisRedisStreamClient : IRedisStreamClient
    {
        public void Subscribe(RedisStreamSettings settings, Func<ResolvedEvent, Task> eventAppeared, CancellationToken cancellationToken)
        {
            //Console.WriteLine("Starting...");
            var host = settings.host;
            var port = settings.Port;
            int timeout = settings.Timeout;
            int index = 1;

            using var client = new TcpClient(host, port);
            var stream = client.GetStream();

            var fullPayload = new StringBuilder();
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = ClientCommands.Subscribe(settings.Stream, settings.BatchSize, index);
                var bytes = Encoding.ASCII.GetBytes(message);
                stream.Write(bytes, 0, bytes.Length);
                do
                {
                    var buffer = new byte[settings.BufferSize];
                    stream.Read(buffer, 0, buffer.Length);

                    var response = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                    fullPayload.Append(response);
                } while (stream.DataAvailable);
                eventAppeared.Invoke(new ResolvedEvent()
                {
                    Data = fullPayload.ToString()
                });
                index += settings.BatchSize;
            }
        }
    }

    public class ResolvedEvent : EventArgs
    {
        public string Data { get; set; }
    }
}