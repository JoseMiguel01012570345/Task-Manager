using TaskManagement.Application.Abstractions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public static class TaskAccess
{
    public static bool CanView(TaskItem task, ICurrentUser user) =>
        user.IsAdministrator ||
        string.Equals(task.AssigneeId, user.UserId, StringComparison.Ordinal) ||
        string.Equals(task.CreatedById, user.UserId, StringComparison.Ordinal);

    public static bool CanModify(TaskItem task, ICurrentUser user) => CanView(task, user);

    public static bool CanAssignTo(ICurrentUser user, string assigneeId) =>
        user.IsAdministrator || string.Equals(assigneeId, user.UserId, StringComparison.Ordinal);
}
