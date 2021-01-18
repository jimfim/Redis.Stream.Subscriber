using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using redis_tcp;

namespace Redis.Stream.Subscriber.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IRedisStreamClient client = new RedisRedisStreamClient();
            client.Subscribe(new RedisStreamSettings()
            {
                host = "localhost",
                Stream = "EventNet:Primary"
            },EventAppeared, new CancellationToken());

            Console.WriteLine("Waiting for Events");
            Console.ReadLine();
        }

        private static Task EventAppeared(ResolvedEvent arg)
        {
            Console.WriteLine(arg.Data);
            return Task.CompletedTask;
        }
    }
}