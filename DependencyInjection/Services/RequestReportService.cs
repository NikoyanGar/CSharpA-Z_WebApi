using System;

namespace DependencyInjection
{
    // Scoped service aggregating multiple lifetimes.
    public sealed class RequestReportService
    {
        private readonly ITransientOperation _transient1;
        private readonly ITransientOperation _transient2;
        private readonly IScopedOperation _scoped;
        private readonly ISingletonOperation _singleton;
        private readonly ITimeProvider _timeProvider;

        public Guid Id { get; } = Guid.NewGuid(); // Its own scoped identity.

        public RequestReportService(
            ITransientOperation transient1,
            ITransientOperation transient2,
            IScopedOperation scoped,
            ISingletonOperation singleton,
            ITimeProvider timeProvider)
        {
            _transient1 = transient1;
            _transient2 = transient2;
            _scoped = scoped;
            _singleton = singleton;
            _timeProvider = timeProvider;
        }

        public DiReport CreateReport() => new(
            Transient1: _transient1.Id,
            Transient2: _transient2.Id,
            Scoped: _scoped.Id,
            Singleton: _singleton.Id,
            ReportServiceInstance: Id,
            UtcNow: _timeProvider.Now
        );
    }

    public sealed record DiReport(
        Guid Transient1,
        Guid Transient2,
        Guid Scoped,
        Guid Singleton,
        Guid ReportServiceInstance,
        DateTimeOffset UtcNow
    );
}