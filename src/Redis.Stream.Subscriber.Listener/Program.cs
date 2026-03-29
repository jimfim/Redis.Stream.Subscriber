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
                Host = "localhost",
                Port = 6379
            });

            Console.WriteLine("\nPress Enter to read backwards from newest entries...");
            Console.ReadLine();

            var backwardEntries = connect.ReadStreamBackwardsAsync("mystream", "-", 10);
            await foreach (var entry in backwardEntries)
            {
                await ProcessEntry(entry);
            }

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
            connect.Close();
        }

        private static async Task ProcessEntry(StreamEntry arg)
        {
            await Console.Out.WriteLineAsync($"message received : {arg.Data}");
        }
    }
}
