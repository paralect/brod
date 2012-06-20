﻿using System;

namespace Brod
{
    public class Message
    {
        /// <summary>
        /// "Magic" number (version of binary format). 
        /// Always 1, because there is only one format for now.
        /// </summary>
        public Byte Magic { get; set; }

        /// <summary>
        /// CRC32 hashing (4 bytes)
        /// </summary>
        public byte[] Crc { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        public byte[] Payload { get; set; }

        /// <summary>
        /// Message length (autocalculated)
        /// </summary>
        public Int32 MessageLength
        {
            get 
            { 
                return 
                  Payload.Length /* size of payload */ +
                  1 /* "magic" byte */ + 
                  4 /* CRC32 hash size */ + 
                  4 /* message length int size */; 
            }
        }

        /// <summary>
        /// Validate message by computing CRC32 hash for payload and comparing with Crc property.
        /// Throws exception if validation fails.
        /// </summary>
        public void Validate()
        {
            using (var crc32 = new Crc32())
            {
                var crc = crc32.ComputeHash(Payload);

                if (!ByteArraysEqual(crc, Crc))
                    throw new Exception("Possibly corrupted message.");
            }            
        }

        /// <summary>
        /// Factory method for creating message with payload
        /// </summary>
        public static Message CreateMessage(byte[] payload)
        {
            using (var crc32 = new Crc32())
            {
                var message = new Message();
                message.Magic = 1;
                message.Payload = payload;
                message.Crc = crc32.ComputeHash(payload);

                return message;
            }
        }

        /// <summary>
        /// Calculate payload length based on message length
        /// </summary>
        public static Int32 CalculatePayloadSize(Int32 messageLength)
        {
            return messageLength - 1 - 4 - 4;
        }

        /// <summary>
        /// Calculate message length based on payload length
        /// </summary>
        public static Int32 CalculateMessageSize(Int32 payloadLength)
        {
            return payloadLength + 1 + 4 + 4;
        }

        /// <summary>
        /// Trivial, yet efficient, byte array comparison
        /// </summary>
        private bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;

            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}