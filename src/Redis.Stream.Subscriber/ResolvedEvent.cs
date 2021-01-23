using System;

namespace Redis.Stream.Subscriber
{
    public class ResolvedEvent : EventArgs
    {
        public string Stream { get; set; }

        public string FieldName { get; set; }

        public string Id { get; set; }
        public string Data { get; set; }
    }
}