using System;

namespace DependencyInjection
{
    public interface IOperation
    {
        Guid Id { get; }
    }

    public interface ITransientOperation : IOperation { }
    public interface IScopedOperation : IOperation { }
    public interface ISingletonOperation : IOperation { }

    public sealed class TransientOperation : ITransientOperation
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    public sealed class ScopedOperation : IScopedOperation
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    public sealed class SingletonOperation : ISingletonOperation
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}