namespace StartupExplained
{
    // Simple service abstractions to show DI lifetimes
    public interface ITimeProvider { DateTime UtcNow { get; } }
}
