using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using zxm.AsyncLock;

namespace zxm.AsyncLock.Tests
{
    public class AsyncLockTest
    {
        private int _counter;
        private object _obj;
        private readonly Locker _lock = new Locker();

        [Fact]
        public void TestLocker()
        {
            _counter = 0;

            Task.WaitAll(GetCounterWithoutLocker(), GetCounterWithoutLocker(), GetCounterWithoutLocker());

            Assert.Equal(_counter, 3);

            _counter = 0;
            _obj = null;

            Task.WaitAll(GetCounterWithLocker(), GetCounterWithLocker(), GetCounterWithLocker());

            Assert.Equal(_counter, 1);
        }

        private async Task<object> GetCounterWithoutLocker()
        {
            if (_obj == null)
            {
                await Task.Delay(500);
                _obj = new object();
                _counter++;
            }
            return _obj;
        }

        private async Task<object> GetCounterWithLocker()
        {
            using (var releaser = await _lock.LockAsync())
            {
                if (_obj == null)
                {
                    await Task.Delay(500);
                    _obj = new object();
                    _counter++;
                }
                return _obj;
            }
        }
    }
}
