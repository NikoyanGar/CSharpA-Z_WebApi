using System;

namespace DependencyInjection
{
    public interface ITimeProvider
    {
        DateTimeOffset Now { get; }
    }

    public sealed class SystemTimeProvider : ITimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}