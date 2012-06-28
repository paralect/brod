using System;

namespace Brod.Producers
{
    /// <summary>
    /// Partitioner route producer requests to selected partition
    /// </summary>
    public interface IPartitioner
    {
        /// <summary>
        /// Selects partition number by key and total number of partitions
        /// </summary>
        /// <returns>An integer between 0 and numberOfPartitions-1</returns>
        Int32 SelectPartition(Object key, Int32 numberOfPartitions);
    }
}