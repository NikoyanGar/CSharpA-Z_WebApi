using System;

namespace DependencyInjection
{
    public interface ISingletonConsumer
    {
        SingletonScopeSampleResult UseScopedSafely();
    }

    public sealed class SingletonConsumer : ISingletonConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISingletonOperation _singletonOperation;

        public SingletonConsumer(IServiceScopeFactory scopeFactory, ISingletonOperation singletonOperation)
        {
            _scopeFactory = scopeFactory;
            _singletonOperation = singletonOperation;
        }

        public SingletonScopeSampleResult UseScopedSafely()
        {
            using var scope = _scopeFactory.CreateScope();
            var scoped = scope.ServiceProvider.GetRequiredService<IScopedOperation>();
            return new SingletonScopeSampleResult(_singletonOperation.Id, scoped.Id);
        }
    }

    public sealed record SingletonScopeSampleResult(Guid SingletonId, Guid FreshScopedId);

    // Anti-pattern: Capturing a scoped service inside a singleton constructor.
    // This would make the scoped instance live for the entire app lifetime (a "captive dependency").
    // public sealed class BadSingletonCapturingScoped
    // {
    //     public IScopedOperation Captured { get; }
    //     public BadSingletonCapturingScoped(IScopedOperation scoped) => Captured = scoped;
    // }
}