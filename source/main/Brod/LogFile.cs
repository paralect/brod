using System.IO;

namespace Brod
{
    public class LogFile
    {
        readonly FileInfo _file;

        public LogFile(string name)
        {
            _file = new FileInfo(name);
        }

        public FileStream OpenForWrite()
        {
            // we allow concurrent reading
            // no more writers are allowed
            return _file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        public FileStream OpenForRead()
        {
            // we allow concurrent writing or reading
            return _file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}