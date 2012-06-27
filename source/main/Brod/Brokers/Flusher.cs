using System.Threading;
using System.Timers;
using Brod.Common.Tasks;
using Brod.Storage;

namespace Brod.Brokers
{
    public class Flusher : ITask
    {
        private readonly BrokerConfiguration _configuration;
        private readonly Store _storage;

        private System.Timers.Timer _timer;
        private CancellationToken _cancellationToken;

        public Flusher(BrokerConfiguration configuration, Store storage)
        {
            _configuration = configuration;
            _storage = storage;
        }

        public void Run(CancellationToken token)
        {
            _cancellationToken = token;
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        public void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_cancellationToken.IsCancellationRequested)
                _timer.Enabled = false;

            // Flushing to disk
            _storage.FlushOnDisk();
        }

        public void Init()
        {
            // nothing to init
        }

        public void Dispose()
        {
            if (_timer != null)
                _timer.Close();
        }
    }
}