using System;
using System.Threading;
using System.Threading.Tasks;

namespace zxm.AsyncLock
{
    public class Locker
    {
        private readonly Semaphore _semaphore;
        private readonly Task<Releaser> _releaser;

        public Locker()
        {
            _semaphore = new Semaphore(1);
            _releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = _semaphore.WaitAsync();
            return wait.IsCompleted ?
                _releaser :
                wait.ContinueWith((_, state) => new Releaser((Locker)state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly Locker _toRelease;

            internal Releaser(Locker toRelease) { _toRelease = toRelease; }

            public void Dispose()
            {
               _toRelease?._semaphore.Release();
            }
        }
    }
}
