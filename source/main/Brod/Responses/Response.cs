using System.IO;

namespace Brod.Responses
{
    public abstract class Response
    {
        public abstract void WriteToStream(Stream stream, BinaryWriter writer);
    }
}