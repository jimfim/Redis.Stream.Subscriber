using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace redis_tcp
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.WriteLine("Starting...");
            var host = "localhost";
            var port = 6379;
            int timeout = 10000;
            string eventStream = "EventNet:Primary";
            //var message = "PING\r\n";
            //var message = "XREAD BLOCK 0 COUNT STREAMS EventNet:Primary $\r\n";
            //var message = $"XRANGE {eventStream} - +\r\n";
            //var message = $"XREAD BLOCK 0 STREAMS {eventStream} $\r\n";
            int batchSize = 1;
            int index = 1;

            StringBuilder fullPayload = new StringBuilder();

            using var client = new TcpClient(host, port);
            var stream = client.GetStream();

            // get stream info
            // stream.Write(Encoding.ASCII.GetBytes($"XINFO STREAM {eventStream}\r\n"));
            // while (stream.DataAvailable && stream.CanRead)
            // {
            //     var buffer = new Byte[1024];
            //     stream.Read(buffer, 0, buffer.Length);
            //     var response = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            //     fullPayload.Append(response);
            // }
            //
            // //catchup
            // var streamInfo = fullPayload.ToString();
            // Console.WriteLine(streamInfo);
            // RespParser parser = new RespParser(streamInfo);
            // var test = await parser.ParseStreamInfo();
            // var max = int.Parse(((object[]) test)[1].ToString());

             while (stream.CanRead)
             {
                fullPayload.Clear();
                var message = $"XREAD BLOCK 0 COUNT {batchSize} STREAMS {eventStream} {index}\r\n";
                var bytes = Encoding.ASCII.GetBytes(message);
                stream.Write(bytes, 0, bytes.Length);
                //while (stream.CanRead)
                //while (stream.CanRead)
                {
                    var buffer = new Byte[1024];
                    stream.Read(buffer, 0, buffer.Length);
                    var response = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                    fullPayload.Append(response);
                }

                Console.WriteLine(fullPayload.ToString());
                index += batchSize;
            }

            // stream


            Console.WriteLine("stopping...");
            stream.Close();
            client.Close();
        }

    }

    public class RespParser
    {
        private TextReader reader;

        public RespParser(string str)
        {
            reader = new StringReader(str);
        }

        public RespParser(Stream stream)
        {
            reader = new StreamReader(stream);
        }

        public object Read()
        {
            switch (reader.Read())
            {
                case '+':
                    return reader.ReadLine();
                case '-':
                    {
                        char[] sep = { ' ' };
                        string[] split = reader.ReadLine().Split(sep, 2);
                        return new Error(split[0], split[1]);
                    }
                case ':':
                    return int.Parse(reader.ReadLine());
                case '$':
                    {
                        int length = int.Parse(reader.ReadLine());
                        if (length < 0)
                        {
                            return null;
                        }

                        char[] buf = new char[length];
                        reader.ReadBlock(buf, 0, length);
                        reader.ReadLine();
                        return (new StringBuilder()).Append(buf).ToString();
                    }
                case '*':
                    {
                        int length = int.Parse(reader.ReadLine());
                        if (length < 0)
                        {
                            return null;
                        }

                        object[] arr = new object[length];
                        for (int i = 0; i < length; i++)
                        {
                            arr[i] = this.Read();
                        }
                        return arr;
                    }
                case -1:
                    // TODO(schoon) - Find a more useful EOF indicator.
                    throw new EndOfStreamException();
                default:
                    return this.Read();
            }
        }

        public async Task<object> ParseStreamInfo()
        {
            char[] next = new char[1];
            if (await reader.ReadAsync(next, 0, 1) == 0)
            {
                // TODO(schoon) - Find a more useful EOF indicator.
                throw new EndOfStreamException();
            }

            switch (next[0])
            {
                case '+':
                    return await reader.ReadLineAsync();
                case '-':
                    {
                        char[] sep = { ' ' };
                        string line = await reader.ReadLineAsync();
                        string[] split = line.Split(sep, 2);
                        return new Error(split[0], split[1]);
                    }
                case ':':
                    return int.Parse(await reader.ReadLineAsync());
                case '$':
                    {
                        int length = int.Parse(await reader.ReadLineAsync());
                        if (length < 0)
                        {
                            return null;
                        }

                        char[] buf = new char[length];
                        await reader.ReadBlockAsync(buf, 0, length);
                        await reader.ReadLineAsync();
                        return (new StringBuilder()).Append(buf).ToString();
                    }
                case '*':
                    {
                        int length = int.Parse(await reader.ReadLineAsync());
                        if (length < 0)
                        {
                            return null;
                        }

                        object[] arr = new object[length];
                        for (int i = 0; i < length; i++)
                        {
                            arr[i] = await this.ParseStreamInfo();
                        }
                        return arr;
                    }
                default:
                    return await this.ParseStreamInfo();
            }
        }

        public struct Error
        {
            public readonly string type;
            public readonly string message;

            public Error(string type, string message)
            {
                this.type = type.ToUpper();
                this.message = message;
            }
        }
    }
}
