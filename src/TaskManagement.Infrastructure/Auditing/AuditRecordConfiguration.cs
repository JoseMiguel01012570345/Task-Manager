using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagement.Infrastructure.Auditing;

internal sealed class AuditRecordConfiguration : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        builder.ToTable("audit_records");
        builder.HasKey(record => record.Id);
        builder.Property(record => record.ActorId).HasMaxLength(128).IsRequired();
        builder.Property(record => record.Action).HasMaxLength(64).IsRequired();
        builder.Property(record => record.Summary).HasMaxLength(1000).IsRequired();
        builder.Property(record => record.CorrelationId).HasMaxLength(128);
        builder.HasIndex(record => record.ResourceId);
        builder.HasIndex(record => record.OccurredAt);
    }
}
