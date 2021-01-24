using System;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Stream.Subscriber.Client
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Waiting for events....");
            var cancellationToken = new CancellationToken();
            var connection = new RedisConnection();

            var connect = connection.Connect(new RedisStreamSettings
            {
                host = "localhost",
                Port = 6379
            });
            
            var entries = connect.ReadStreamAsync("mystream", 0,new SubscriptionSettings()
            {
                BatchSize = 2
            }, cancellationToken);

            await foreach (var entry in entries.WithCancellation(cancellationToken))
            {
                await ProcessEntry(entry);
            }
            
            Console.ReadLine();
            connect.Close();
        }

        private static async Task ProcessEntry(StreamEntry arg)
        {
            await Console.Out.WriteLineAsync($"=== {arg.Id} ==== ");
            await Console.Out.WriteLineAsync(arg.FieldName);
            await Console.Out.WriteLineAsync(arg.Data);
        }
    }
}