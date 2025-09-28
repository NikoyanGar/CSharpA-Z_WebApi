namespace MiddlewarePipeline.Middlewares
{
    public class CounterState
    {
        private long _count;
        public long RequestCount => Interlocked.Read(ref _count);
        public void Increment() => Interlocked.Increment(ref _count);
    }
}