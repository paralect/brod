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
}