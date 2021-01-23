using System;
using System.Net.Sockets;

namespace Redis.Stream.Subscriber
{
    public class RedisConnection : IRedisConnection
    {
        private TcpClient client;
        
        public IRedisStreamClient Connect(RedisStreamSettings settings)
        {
            client = new TcpClient(settings.host, settings.Port);
            var stream = client.GetStream();
            return new RedisRedisStreamClient(stream);
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}