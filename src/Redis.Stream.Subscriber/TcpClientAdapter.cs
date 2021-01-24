using System.Net.Sockets;

namespace Redis.Stream.Subscriber
{
    public class TcpClientAdapter: ITcpClient
    {
        private readonly TcpClient _wrappedClient;
        
        public TcpClientAdapter(TcpClient client)
        {
            _wrappedClient = client;
        }

        public NetworkStream GetStream()
        {
            return _wrappedClient.GetStream();
        }

        public void Close()
        {
            _wrappedClient.Close();
        }
    }
}