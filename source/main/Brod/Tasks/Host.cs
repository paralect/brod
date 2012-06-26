using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Brod.Common;
using Brod.Tasks.Abstract;
using Brod.Tasks.Events;

namespace Brod.Tasks
{
    /// <summary>
    /// Host that starts, waits or cancells execution of Tasks
    /// </summary>
    public class Host : IDisposable
    {
        /// <summary>
        /// Tasks that are managed by host
        /// </summary>
        private readonly ICollection<ITask> _tasks;

        /// <summary>
        /// This stack tracks disposable objects in order to dispose them on shutdown
        /// </summary>
        private readonly Stack<IDisposable> _disposables = new Stack<IDisposable>();

        /// <summary>
        /// Names of tasks
        /// </summary>
        private readonly String[] _taskNames;

        /// <summary>
        /// Constructs Host with specified non-empty collection of tasks
        /// </summary>
        public Host(ICollection<ITask> tasks)
        {
            _tasks = tasks;

            // At least one process should be registered
            if (_tasks.Count == 0)
                throw new InvalidOperationException(String.Format("There were no instances of '{0}' registered", typeof(ITask).Name));

            // Build list of tasks names
            _taskNames = _tasks
                .Select(p => String.Format("{0}({1:X8})", p.GetType().Name, p.GetHashCode()))
                .ToArray();

            // Register disposables
            foreach (var process in _tasks)
                _disposables.Push(process);

            // Initialize Tasks
            Initialize();
        }

        /// <summary>
        /// Constructor overload 
        /// </summary>
        public Host(params ITask[] tasks) : this((ICollection<ITask>)tasks)
        {
            
        }

        /// <summary>
        /// Start engine host.
        /// This will start each task in registered in this host.
        /// </summary>
        /// <param name="token">
        /// A CancellationToken to observe while waiting for the tasks to complete.
        /// </param>
        /// <param name="timeout">
        /// The number of milliseconds to wait, or Infinite (-1) to wait indefinitely
        /// </param>
        public Task Start(CancellationToken token, Int32 timeout)
        {
            return Task.Factory.StartNew(() =>
            {
                var watch = Stopwatch.StartNew();

                // Try to start all tasks
                var tasks = _tasks
                    .Select(p => Task.Factory.StartNew(() => p.Run(token)))
                    .ToArray();

                // Engine started
                SystemInformer.Notify(new EngineStarted(_taskNames));

                try
                {
                    // Wait for all tasks to be either completed or canceled 
                    Task.WaitAll(tasks, timeout, token);
                }
                catch (OperationCanceledException)
                {
                    // Do nothing
                }

                // Engine stopped
                SystemInformer.Notify(new EngineStopped(watch.Elapsed));
            });
        }

        /// <summary>
        /// Initialize Engine Host
        /// </summary>
        internal void Initialize()
        {
            // About to initialize
            SystemInformer.Notify(new EngineInitializing());

            // Initialize all tasks
            foreach (var process in _tasks)
                process.Init();

            // All process initialized
            SystemInformer.Notify(new EngineInitialized());
        }

        /// <summary>
        /// Try to dispose all tasks
        /// </summary>
        public void Dispose()
        {
            while (_disposables.Count > 0)
            {
                try
                {
                    _disposables.Pop().Dispose();
                }
                catch
                {
                    // Suppressing all exceptions because we unable 
                    // to handle them correctly when host is shutdowning  
                }
            }            
        }
    }
}
