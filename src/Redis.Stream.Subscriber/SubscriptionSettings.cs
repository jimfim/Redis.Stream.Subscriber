using System;

namespace Redis.Stream.Subscriber
{
    /// <summary>
    /// Configuration settings for stream subscription behavior.
    /// </summary>
    public class SubscriptionSettings
    {
        private int _bufferSize = 1024;
        private int _batchSize = 1;

        /// <summary>
        /// Size of the buffer used for reading data from Redis. Defaults to 1024 bytes.
        /// </summary>
        public int BufferSize
        {
            get => _bufferSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("BufferSize must be greater than zero", nameof(BufferSize));
                }
                _bufferSize = value;
            }
        }

        /// <summary>
        /// Number of stream entries to read in each batch. Defaults to 1.
        /// </summary>
        public int BatchSize
        {
            get => _batchSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("BatchSize must be greater than zero", nameof(BatchSize));
                }
                _batchSize = value;
            }
        }

        /// <summary>
        /// Validates that all settings are within acceptable ranges.
        /// Throws ArgumentException if validation fails.
        /// </summary>
        public void Validate()
        {
            if (BufferSize <= 0)
            {
                throw new ArgumentException("BufferSize must be greater than zero", nameof(BufferSize));
            }

            if (BatchSize <= 0)
            {
                throw new ArgumentException("BatchSize must be greater than zero", nameof(BatchSize));
            }
        }
    }
}