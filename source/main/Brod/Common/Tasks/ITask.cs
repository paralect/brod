using System;
using System.Threading;

namespace Brod.Common.Tasks
{
    public interface ITask : IDisposable
    {
        void Init();
        void Run(CancellationToken token);
    }
}