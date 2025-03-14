using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Core.Scheduling.SqlServer.Internal.Data;

public class SqlServerInternalMessageEntityTypeConfiguration : IEntityTypeConfiguration<InternalMessage>
{
    public void Configure(EntityTypeBuilder<InternalMessage> builder)
    {
        builder.ToTable("InternalMessages", SqlServerInternalMessageDbContext.DefaultSchema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.OccurredOn)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Data)
            .IsRequired();

        builder.Property(x => x.ProcessedOn)
            .IsRequired(false);
    }
}
