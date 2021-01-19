using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Stream.Subscriber.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Waiting for events....");
            IRedisStreamClient client = new RedisRedisStreamClient();
            client.Subscribe(new RedisStreamSettings()
            {
                host = "localhost",
                Stream = "EventNet:Primary"
            },EventAppeared, new CancellationToken());

            Console.ReadLine();
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