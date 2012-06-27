using System.IO;
using Brod.Common;

namespace Brod.Contracts.Responses
{
    public abstract class Response
    {
        public abstract void WriteToStream(BinaryStream stream);
    }
}