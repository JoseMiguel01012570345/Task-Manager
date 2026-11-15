using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Infrastructure.Persistence;

internal sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title).HasMaxLength(TaskItem.TitleMaxLength).IsRequired();
        builder.Property(task => task.Description).HasMaxLength(TaskItem.DescriptionMaxLength).IsRequired();
        builder.Property(task => task.AssigneeId).HasMaxLength(TaskItem.UserIdMaxLength).IsRequired();
        builder.Property(task => task.CreatedById).HasMaxLength(TaskItem.UserIdMaxLength).IsRequired();
        builder.Property(task => task.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(task => task.Version).IsRowVersion();

        builder.HasIndex(task => task.Status);
        builder.HasIndex(task => task.AssigneeId);
        builder.HasIndex(task => task.DueDate);

        builder.HasQueryFilter(task => !task.IsDeleted);
    }
}
