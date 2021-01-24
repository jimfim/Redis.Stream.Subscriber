using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Stream.Subscriber.Publisher
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("connecting to redis");
            ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost");
            
            var db = redis.GetDatabase();
            var count = 1;
            var streamName = "mystream";
            Console.WriteLine($"Press Enter to send data to the [{streamName}] stream....");
            while (true)
            {
                Console.ReadLine();
                await db.StreamAddAsync(streamName, "foo_name", $"bar_{count}", $"{count}-0");
                Console.Write($"message sent : {count}");
                count++;
            }
        }
    }
}