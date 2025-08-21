using BT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BT.Infrastructure.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(250);
            
        builder.Property(c => c.Description)
            .HasMaxLength(1000);
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);
        
        builder.Property(e => e.CreatedBy)
            .HasMaxLength(450)
            .IsRequired(false);
        
        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(450)
            .IsRequired(false);
        
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.CreatedAt);
    }
}