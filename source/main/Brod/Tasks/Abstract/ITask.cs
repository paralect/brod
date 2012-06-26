using System;
using System.Threading;

namespace Brod.Tasks.Abstract
{
    public interface ITask : IDisposable
    {
        void Init();
        void Run(CancellationToken token);
    }
}