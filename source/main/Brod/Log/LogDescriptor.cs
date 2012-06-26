using System;
using Brod.Utilities;

namespace Brod.Log
{
    public class LogDescriptor
    {
        public String Topic { get; set; }
        public Int32 Partition { get; set; }
        
        public override bool Equals(object obj)
        {
            var descriptor = (LogFileDescriptor)obj;
            if (String.CompareOrdinal(Topic, descriptor.Topic) != 0)
                return false;

            if (Partition != descriptor.Partition)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return HashCodeUtils.Compute(Topic, Partition);
        }         
    }
}