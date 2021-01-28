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
            var connection = new RedisConnection();

            var connect = connection.Connect(new RedisStreamSettings
            {
                host = "localhost",
                Port = 6379
            });

            uint startingIndex = 0;
            var entries = connect.ReadStreamAsync("mystream", startingIndex);
            await foreach (var entry in entries)
            {
                await ProcessEntry(entry);
            }
            
            Console.ReadLine();
            connect.Close();
        }

        private static async Task ProcessEntry(StreamEntry arg)
        {
            await Console.Out.WriteLineAsync($"message received : {arg.Data}");
        }
    }
}