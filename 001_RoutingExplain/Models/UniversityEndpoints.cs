namespace _001_RoutingExplain.Models
{
    public static class UniversityEndpoints
    {
        public static void MapUniversityEndpoints(this IEndpointRouteBuilder routes)
        {
            // Create a group of endpoints under the "/api/University" route
            var group = routes.MapGroup("/api/University").WithTags(nameof(University));

            // Map a GET request to the root endpoint ("/api/University")
            group.MapGet("/", () =>
            {
                // Return an array containing a new instance of the University class
                return new[] { new University() };
            })
            .WithName("GetAllUniversities")
            .WithOpenApi();

            // Map a GET request to the endpoint with a parameter ("/api/University/{id}")
            group.MapGet("/{id}", (int id) =>
            {
                // This endpoint is currently commented out, so it doesn't return anything
                // It would typically return a University object with the specified ID
            })
            .WithName("GetUniversityById")
            .WithOpenApi();

            // Map a PUT request to the endpoint with a parameter ("/api/University/{id}")
            group.MapPut("/{id}", (int id, University input) =>
            {
                // Return a response indicating that the update was successful
                return TypedResults.NoContent();
            })
            .WithName("UpdateUniversity")
            .WithOpenApi();

            // Map a POST request to the root endpoint ("/api/University")
            group.MapPost("/", (University model) =>
            {
                // This endpoint is currently commented out, so it doesn't return anything
                // It would typically create a new University object with the provided model data
            })
            .WithName("CreateUniversity")
            .WithOpenApi();

            // Map a DELETE request to the endpoint with a parameter ("/api/University/{id}")
            group.MapDelete("/{id}", (int id) =>
            {
                // This endpoint is currently commented out, so it doesn't return anything
                // It would typically delete the University object with the specified ID
            })
            .WithName("DeleteUniversity")
            .WithOpenApi();
        }
    }
}


