using System;
using System.Net.Sockets;

namespace Redis.Stream.Subscriber
{
    public class RedisConnection : IRedisConnection
    {
        private ITcpClient _client;
        
        public IRedisStreamClient Connect(RedisStreamSettings settings)
        {
            _client = new TcpClientAdapter(new TcpClient(settings.host, settings.Port));
            var stream = _client.GetStream();
            return new RedisRedisStreamClient(stream);
        }
        
        public IRedisStreamClient Connect(RedisStreamSettings settings, ITcpClient client)
        {
            _client = client;
            var stream = _client.GetStream();
            return new RedisRedisStreamClient(stream);
        }

        public void Close()
        {
            _client.Close();
        }
    }
}