using Microsoft.AspNetCore.Mvc;

namespace DependencyInjection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiDemoController : ControllerBase
    {
        private readonly RequestReportService _reportService;
        private readonly ISingletonConsumer _singletonConsumer;
        private readonly ISingletonOperation _singletonOperation;

        public DiDemoController(
            RequestReportService reportService,
            ISingletonConsumer singletonConsumer,
            ISingletonOperation singletonOperation)
        {
            _reportService = reportService;
            _singletonConsumer = singletonConsumer;
            _singletonOperation = singletonOperation;
        }

        // Shows all lifetimes within THIS request scope.
        [HttpGet("report")]
        public ActionResult<object> GetReport()
        {
            var report = _reportService.CreateReport();
            return Ok(new
            {
                Message = "Compare values across multiple requests to see lifetime behavior.",
                report.Transient1,
                report.Transient2,
                report.Scoped,
                report.Singleton,
                report.ReportServiceInstance,
                report.UtcNow
            });
        }

        // Shows how a singleton can safely obtain a fresh scoped instance.
        [HttpGet("singleton-uses-scope")]
        public ActionResult<object> SingletonUsesScope()
        {
            var result = _singletonConsumer.UseScopedSafely();
            return Ok(new
            {
                Explanation = "Singleton ID stays constant; scoped ID changes per request (and per created scope).",
                SingletonIdFromControllerInjection = _singletonOperation.Id,
                SingletonIdFromConsumer = result.SingletonId,
                FreshScopedIdFromCreatedScope = result.FreshScopedId
            });
        }
    }
}