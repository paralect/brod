using System;

namespace Brod.Producers
{
    /// <summary>
    /// Partitioner route producer requests to partition number returned from SelectPartition(key, numberOfPartitions)
    /// </summary>
    public interface IPartitioner
    {
        /// <summary>
        /// Selects partition number by key and total number of partitions
        /// </summary>
        Int32 SelectPartition(Object key, Int32 numberOfPartitions);
    }
}