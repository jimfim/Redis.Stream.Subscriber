using System;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Stream.Subscriber
{
    public interface IRedisStreamClient
    {
        Task Subscribe(RedisStreamSettings settings, Func<ResolvedEvent, Task> eventAppeared,
                CancellationToken cancellationToken);

        void Close();

    }

    public class ResolvedEvent : EventArgs
    {
        public string Stream { get; set; }

        public string FieldName { get; set; }

        public string Id { get; set; }
        public string Data { get; set; }
    }
}