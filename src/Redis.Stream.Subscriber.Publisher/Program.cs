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
            var streamName = "mystream";
            var checkpointKey = $"{streamName}-checkpoint";
            var db = redis.GetDatabase();
            int.TryParse(await db.StringGetAsync(checkpointKey), out var count);
            count++;
            Console.WriteLine($"Press Enter to send data to the [{streamName}] stream....");
            while (true)
            {
                Console.ReadLine();
                var transaction = db.CreateTransaction();
                transaction.StreamAddAsync(streamName, "foo_name", $"bar_{count}", $"{count}-0");
                transaction.StringIncrementAsync(checkpointKey);
                await transaction.ExecuteAsync();
                Console.Write($"message sent : {count}");
                count++;
            }
        }
    }
}