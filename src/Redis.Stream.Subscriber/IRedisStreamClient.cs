using System;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Stream.Subscriber
{
    public interface IRedisStreamClient
    {
        Task ReadStreamEventsForwardAsync(string streamName, long lastCheckpoint,
            SubscriptionSettings settings,
            Func<ResolvedEvent, Task> eventAppeared,
            CancellationToken cancellationToken);
        
        Task ReadStreamEventsForwardAsync(string streamName, long lastCheckpoint,
            Func<ResolvedEvent, Task> eventAppeared,
            CancellationToken cancellationToken);

        void Close();
    }
}