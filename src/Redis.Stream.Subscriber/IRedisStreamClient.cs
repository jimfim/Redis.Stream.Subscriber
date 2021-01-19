using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace redis_tcp
{
    public interface IRedisStreamClient
    {
        Task Subscribe(RedisStreamSettings settings, Func<ResolvedEvent, Task> eventAppeared,
                CancellationToken cancellationToken);

    }

    public class ResolvedEvent : EventArgs
    {
        public string Stream { get; set; }

        public string FieldName { get; set; }

        public string Id { get; set; }
        public string Data { get; set; }
    }
}