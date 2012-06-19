using System;

namespace Brod
{
    public class Message
    {
        /// <summary>
        /// Message length
        /// </summary>
        public Int32 Length { get; set; }

        /// <summary>
        /// Magic value 
        /// </summary>
        public Byte Magic { get; set; }

        /// <summary>
        /// SHA1 hashing
        /// </summary>
        public Int32 Crc { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        public byte[] Payload { get; set; }
    }
}