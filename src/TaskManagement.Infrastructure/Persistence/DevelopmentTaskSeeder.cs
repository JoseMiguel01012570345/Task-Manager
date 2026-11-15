using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Infrastructure.Persistence;

public static class DevelopmentTaskSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Tasks.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        db.Tasks.Add(TaskItem.Create(
            "Revisar el tablero",
            "Tarea de ejemplo solo para desarrollo.",
            "dev-user",
            "dev-user",
            now.AddDays(7),
            now));

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
