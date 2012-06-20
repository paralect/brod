﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Brod
{
    public class MessagesBlock
    {
        public Int32 Length { get; set; }
        public byte[] Data { get; set; }

        /// <summary>
        /// Read messages from this message block
        /// </summary>
        public IEnumerable<Message> ReadMessages()
        {
            using (var reader = new MessageReader(new MemoryStream(Data)))
            {
                Message message;
                while ((message = reader.ReadMessage()) != null)
                    yield return message;
            }
        }
    }
}