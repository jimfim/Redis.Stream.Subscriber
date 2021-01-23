using System;
using StackExchange.Redis;

namespace Redis.Stream.Subscriber.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("connecting to redis");
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            
            var db = redis.GetDatabase();
            var count = 1;
            var streamName = "mystream";
            Console.WriteLine($"Press Enter to send data to the [{streamName}] stream....");
            while (true)
            {
                Console.ReadLine();
                db.StreamAdd(streamName, "foo_name", $"bar_{count}", $"{count}-0");
                Console.Write($"message sent : {count}");
                count++;
            }
        }
    }
}