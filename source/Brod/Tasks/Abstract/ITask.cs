using System;
using System.Threading;

namespace Brod
{
    public interface ITask : IDisposable
    {
        void Init();
        void Run(CancellationToken token);
    }
}