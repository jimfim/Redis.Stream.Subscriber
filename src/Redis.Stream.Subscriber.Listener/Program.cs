using System;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Stream.Subscriber.Client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Waiting for events....");
            var cancellationToken = new CancellationToken();
            var connection = new RedisConnection();

            var connect = connection.Connect(new RedisStreamSettings
            {
                host = "localhost",
                Port = 6379
            });
            
            connect.ReadStreamEventsForwardAsync("mystream", 0, EventAppeared, cancellationToken);
            Console.ReadLine();
            connect.Close();
        }

        private static async Task EventAppeared(ResolvedEvent arg)
        {
            await Console.Out.WriteLineAsync($"=== {arg.Id} ==== ");
            await Console.Out.WriteLineAsync(arg.Stream);
            await Console.Out.WriteLineAsync(arg.FieldName);
            await Console.Out.WriteLineAsync(arg.Data);
        }
    }
}