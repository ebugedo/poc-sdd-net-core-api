using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using poc_sdd_net_core_api.Domain.Entities;

namespace poc_sdd_net_core_api.Infrastructure.Data.Configurations;

public class SectorConfiguration : IEntityTypeConfiguration<Sector>
{
    public void Configure(EntityTypeBuilder<Sector> builder)
    {
        builder.ToTable("sectors");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasDatabaseName("idx_sectors_name");

        // Catalogo de solo lectura: 7 filas con id fijo (BR-016)
        builder.HasData(Sector.DefaultCatalog);
    }
}
