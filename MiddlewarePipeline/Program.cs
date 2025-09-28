using MiddlewarePipeline.Middlewares;

namespace MiddlewarePipeline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Services
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddSingleton<CounterState>();
            builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

            var app = builder.Build();

            // ------------------------------------------------------------------
            // 1. Global inline middleware (demonstrates simplest Use overload)
            // ------------------------------------------------------------------
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[Inline #1] Incoming: {context.Request.Method} {context.Request.Path}");
                await next();
                Console.WriteLine($"[Inline #1] Outgoing: {context.Response.StatusCode}");
            });

            // ------------------------------------------------------------------
            // 2. Custom middleware via generic Use<T>
            // ------------------------------------------------------------------
            app.Use<CounterMiddleware>(); // increments a request counter

            // ------------------------------------------------------------------
            // 3. Custom middleware registered with extension method
            // ------------------------------------------------------------------
            app.UseRequestTiming();

            // ------------------------------------------------------------------
            // 4. Branching with Map (does NOT return to parent pipeline)
            //    All requests whose path starts with /branch go ONLY through this branch.
            // ------------------------------------------------------------------
            app.Map("/branch", branchApp =>
            {
                branchApp.Use(async (ctx, next) =>
                {
                    Console.WriteLine("[Branch:/branch] Before next");
                    await next();
                    Console.WriteLine("[Branch:/branch] After next");
                });

                branchApp.Map("/branch/deep", deep =>
                {
                    deep.Run(async ctx =>
                    {
                        await ctx.Response.WriteAsync("Deep branch terminal (/branch/deep)\n");
                    });
                });

                // Terminal for /branch (excluding deeper matched ones caught above)
                branchApp.Run(async ctx =>
                {
                    await ctx.Response.WriteAsync("Branch terminal (/branch)\n");
                });
            });

            // ------------------------------------------------------------------
            // 5. Conditional branching with MapWhen (predicate, no return)
            // ------------------------------------------------------------------
            app.MapWhen(
                ctx => ctx.Request.Query.ContainsKey("mapwhen"),
                branch =>
                {
                    branch.Run(async ctx =>
                    {
                        await ctx.Response.WriteAsync("MapWhen branch terminal. Query contained 'mapwhen'.\n");
                    });
                });

            // ------------------------------------------------------------------
            // 6. Conditional branching with UseWhen (branch then returns)
            // ------------------------------------------------------------------
            app.UseWhen(
                ctx => ctx.Request.Query.ContainsKey("usewhen"),
                branch =>
                {
                    branch.Use(async (ctx, next) =>
                    {
                        ctx.Response.Headers.Append("X-UseWhen", "Applied");
                        await next();
                    });
                });

            // ------------------------------------------------------------------
            // 7. Short-circuit middleware (stops pipeline for specific path)
            // ------------------------------------------------------------------
            app.UseMiddleware<ShortCircuitMiddleware>();

            // ------------------------------------------------------------------
            // 8. Request header mutator
            // ------------------------------------------------------------------
            app.UseMiddleware<HeaderMutationMiddleware>();

            // ------------------------------------------------------------------
            // 9. Response body wrapper (captures & appends info)
            // ------------------------------------------------------------------
            app.UseMiddleware<ResponseWrappingMiddleware>();

            // ------------------------------------------------------------------
            // 10. Routing + Endpoint execution
            // ------------------------------------------------------------------
            app.UseRouting();

            // (Authentication placeholder if needed)
            app.UseAuthorization();

            // ------------------------------------------------------------------
            // 11. Endpoints (classic UseEndpoints style)
            // ------------------------------------------------------------------
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                // Minimal API endpoints
                endpoints.MapGet("/hello", async ctx =>
                {
                    await ctx.Response.WriteAsync("Hello from MapGet endpoint\n");
                });

                endpoints.MapPost("/echo", async ctx =>
                {
                    ctx.Response.ContentType = "text/plain";
                    using var reader = new StreamReader(ctx.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    await ctx.Response.WriteAsync($"You posted: {body}\n");
                });

                // Named endpoint
                endpoints.MapGet("/counter", (CounterState state) =>
                {
                    return Results.Ok(new { state.RequestCount });
                }).WithName("CounterEndpoint");
            });

            // ------------------------------------------------------------------
            // 12. Map + Run (terminal inside mapping)
            // ------------------------------------------------------------------
            app.Map("/terminal-run", mapped =>
            {
                mapped.Run(async ctx =>
                {
                    await ctx.Response.WriteAsync("This is a terminal Run() inside Map('/terminal-run')\n");
                });
            });

            // ------------------------------------------------------------------
            // 13. Fallback Terminal (Run) - executes only if nothing else handled
            // ------------------------------------------------------------------
            app.Run(async ctx =>
            {
                ctx.Response.ContentType = "text/plain";
                await ctx.Response.WriteAsync("Fallback terminal middleware (no route matched)\n");
            });

            app.Run();
        }
    }
}
