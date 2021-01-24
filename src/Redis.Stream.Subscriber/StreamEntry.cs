using System;

namespace Redis.Stream.Subscriber
{
    public class StreamEntry : EventArgs
    {
        public string FieldName { get; set; }
        public string Id { get; set; }
        public string Data { get; set; }
    }
}