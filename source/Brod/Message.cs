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
        /// SHA1 hashing (4 bytes)
        /// </summary>
        public byte[] Crc { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        public byte[] Payload { get; set; }
    }
}