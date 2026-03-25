using System;
using System.ComponentModel.DataAnnotations;

namespace Redis.Stream.Subscriber
{
    /// <summary>
    /// Configuration settings for Redis connection.
    /// </summary>
    public class RedisStreamSettings
    {
        [Obsolete("This property has been removed. Use Host instead.", error: true)]
        public string host => throw new InvalidOperationException("Use Host property");

        /// <summary>
        /// The Redis server hostname or IP address. Required.
        /// </summary>
        public string Host { get; set; } = null!;

        /// <summary>
        /// The Redis server port. Defaults to 6379.
        /// </summary>
        public int Port { get; set; } = 6379;

        /// <summary>
        /// Connection timeout in milliseconds. Defaults to 100ms.
        /// </summary>
        public int Timeout { get; set; } = 100;

        /// <summary>
        /// Validates the configuration settings before use.
        /// Throws ArgumentException if validation fails.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Host))
            {
                throw new ArgumentException("Host cannot be null or empty", nameof(Host));
            }

            if (Port < 1 || Port > 65535)
            {
                throw new ArgumentException($"Port must be between 1 and 65535, got {Port}", nameof(Port));
            }

            if (Timeout <= 0)
            {
                throw new ArgumentException("Timeout must be greater than zero", nameof(Timeout));
            }
        }
    }
}
