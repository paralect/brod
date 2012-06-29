using System;

namespace Brod.Producers
{
    /// <summary>
    /// DefaultPartitioner uses hash code of key (if key was provided)
    /// or just random number (if key wasn't provided)
    /// 
    /// That means, that Producer requests with the same key go to the same partition.
    /// </summary>
    public class DefaultPartitioner : IPartitioner
    {
        private readonly Random _random = new Random();

        public int SelectPartition(object key, int numberOfPartitions)
        {
            if (key == null)
                return _random.Next(numberOfPartitions);
            
            return Math.Abs(key.GetHashCode()) % numberOfPartitions;
        }
    }

    /// <summary>
    /// Interprets key as partition number
    /// </summary>
    public class DirectPartitioner : IPartitioner
    {
        public int SelectPartition(object key, int numberOfPartitions)
        {
            if (key == null)
                throw new Exception("DirectPartitioner cannot interpret null keys. Use Int32 key.");

            if (!(key is Int32))
                throw new Exception("DirectPartitioner supports only Int32 keys.");

            var partitionNumber = (Int32) key;

            if (partitionNumber < 0 || partitionNumber >= numberOfPartitions)
                throw new Exception("DirectPartitioner failed to select partition, because specified partition was outside of valid range.");

            return partitionNumber;
        }
    }
}