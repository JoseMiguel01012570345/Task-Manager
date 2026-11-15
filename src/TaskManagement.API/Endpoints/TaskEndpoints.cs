using TaskManagement.API.Contracts;
using TaskManagement.Application.Tasks;

namespace TaskManagement.API.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/tasks").WithTags("Tasks");

        group.MapPost("/", async (
            CreateTaskRequest request,
            CreateTask.Handler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(
                new CreateTask.Command(request.Title, request.Description, request.AssigneeId, request.DueDate),
                cancellationToken).ConfigureAwait(false);

            var response = TaskResponse.From(result.Task);
            return Results.Created($"/api/v1/tasks/{response.Id}", response);
        });

        return app;
    }
}
