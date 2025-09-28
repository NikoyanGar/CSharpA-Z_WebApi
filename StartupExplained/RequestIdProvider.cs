namespace StartupExplained
{
    public sealed class RequestIdProvider : IRequestIdProvider { public Guid Id { get; } = Guid.NewGuid(); }
}

