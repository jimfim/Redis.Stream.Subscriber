using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Stream.Subscriber
{
    public interface IRedisStreamClient
    {
        IAsyncEnumerable<StreamEntry> ReadStreamAsync(string streamName, uint lastCheckpoint, SubscriptionSettings settings, CancellationToken cancellationToken = default);
        
        IAsyncEnumerable<StreamEntry> ReadStreamAsync(string streamName, uint lastCheckpoint, CancellationToken cancellationToken = default);
        
        void Close();
    }
}