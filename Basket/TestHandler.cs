using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket
{
    public sealed class TestHandler<T> : IHandle<T>, IDisposable
    {
        private readonly BlockingCollection<T> _messages = new();

        public void Assert(Action<T> assert, TimeSpan timeout)
        {
            var exceptions = new List<Exception>();
            while (_messages.TryTake(out var result, timeout))
            {
                try
                {
                    assert(result);
                    return;
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            throw exceptions.Any() ? new AggregateException(exceptions) : new TimeoutException();
        }

        public void Assert(Action<T> assert) => 
            Assert(assert, TimeSpan.FromSeconds(60));

        async Task IHandle<T>.Handle(T body)
        {
            _messages.Add(body);
            await Task.CompletedTask;
        }

        void IDisposable.Dispose() => _messages.Dispose();
    }
}