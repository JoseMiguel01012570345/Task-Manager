using TaskManagement.API.Contracts;
using TaskManagement.Application.Tasks;

namespace TaskManagement.API.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/tasks").WithTags("Tasks").RequireAuthorization("TaskAccess");

        group.MapGet("/", async (
            string? status,
            string? assigneeId,
            int? page,
            int? pageSize,
            ListTasks.Handler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(
                new ListTasks.Query(status, assigneeId, page, pageSize),
                cancellationToken).ConfigureAwait(false);

            return Results.Ok(new TaskPageResponse(
                result.Items.Select(TaskResponse.From).ToArray(),
                result.Page,
                result.PageSize,
                result.TotalCount));
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetTask.Handler handler,
            CancellationToken cancellationToken) =>
        {
            var task = await handler.Handle(id, cancellationToken).ConfigureAwait(false);
            return Results.Ok(TaskResponse.From(task));
        });

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

        group.MapPatch("/{id:guid}/status", async (
            Guid id,
            ChangeTaskStatusRequest request,
            ChangeTaskStatus.Handler handler,
            CancellationToken cancellationToken) =>
        {
            var task = await handler.Handle(
                new ChangeTaskStatus.Command(id, request.Status, request.Version),
                cancellationToken).ConfigureAwait(false);
            return Results.Ok(TaskResponse.From(task));
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            uint? version,
            DeleteTask.Handler handler,
            CancellationToken cancellationToken) =>
        {
            await handler.Handle(new DeleteTask.Command(id, version), cancellationToken).ConfigureAwait(false);
            return Results.NoContent();
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateTaskRequest request,
            UpdateTask.Handler handler,
            CancellationToken cancellationToken) =>
        {
            var task = await handler.Handle(
                new UpdateTask.Command(id, request.Title, request.Description, request.AssigneeId, request.DueDate, request.Version),
                cancellationToken).ConfigureAwait(false);

            return Results.Ok(TaskResponse.From(task));
        });

        return app;
    }
}
