using System.IO;

namespace Brod.Contracts.Responses
{
    public abstract class Response
    {
        public abstract void WriteToStream(Stream stream, BinaryWriter writer);
    }
}