using System.Net.Sockets;

namespace Redis.Stream.Subscriber
{
    /// <summary>
    /// Allows us to mock the TCP client in unit tests
    /// </summary>
    public interface ITcpClient
    {
        NetworkStream GetStream();

        void Close();
    }
}