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
            string checkpointKey = $"{streamName}-checkpoint";
            var db = redis.GetDatabase();
            var value = await db.StringGetAsync(checkpointKey);
            int.TryParse(value.ToString(),
                         out var count);
            count++;
            Console.WriteLine($"Send data to the [{streamName}] stream....");

            while (true)
            {
                using (var transaction = db.CreateTransaction())
                {
                    await transaction.StreamAddAsync(streamName, "foo_name", $"bar_{count}", $"{count}-0");
                    await transaction.StringIncrementAsync(checkpointKey);
                    await transaction.ExecuteAsync();
                }
                Console.WriteLine($"message sent : {count}");
                count++;
                Thread.Sleep(2000);
            }
        }
    }
}
