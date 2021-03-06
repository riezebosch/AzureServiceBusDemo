using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using EDA.TestAdapters;

namespace EDA.EventHubs
{
    internal class TestProcessor<T> : ITestProcessor<T>
    {
        private readonly IProcessor _processor;
        private readonly Subscriber<T> _subscriber = new();

        public TestProcessor(EventProcessorClient processor, string @event) =>
            _processor = new Processor<T>(processor, _subscriber, @event);

        public async Task Start() => 
            await _processor.Start();

        public ValueTask DisposeAsync() => 
            _processor.DisposeAsync();

        public Task Assert(Action<T> assert, TimeSpan timeout) => 
            _subscriber.Assert(assert, timeout);

        public Task Assert(Action<T> assert) => 
            _subscriber.Assert(assert);
    }
}