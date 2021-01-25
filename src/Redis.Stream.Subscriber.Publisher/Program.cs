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
            Console.WriteLine($"Send data to the [{streamName}] stream....");
            
            while (true)
            {
                var transaction = db.CreateTransaction();
                
#pragma warning disable 4014
                transaction.StreamAddAsync(streamName, "foo_name", $"bar_{count}", $"{count}-0");
                transaction.StringIncrementAsync(checkpointKey);
#pragma warning restore 4014                
                
                await transaction.ExecuteAsync();
                Console.WriteLine($"message sent : {count}");
                count++;
                Thread.Sleep(2000);
            }
        }
    }
}