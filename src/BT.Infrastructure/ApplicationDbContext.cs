using System.Reflection;
using System.Security.Claims;
using BT.Domain.Entities;
using BT.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BT.Infrastructure;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options, 
    IHttpContextAccessor httpContextAccessor) 
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Company> Companies { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleAuditProperties();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    public override int SaveChanges()
    {
        HandleAuditProperties();
        return base.SaveChanges();
    }

    private void HandleAuditProperties()
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    
                    // Prevent modification of creation audit fields
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                    break;
            }
        }
        
        foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    
                    // Prevent modification of creation audit fields
                    entry.Property(nameof(ApplicationUser.CreatedAt)).IsModified = false;
                    break;
            }
        }
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        foreach(var entity in builder.Model.GetEntityTypes())
        {
            // Replace table names
            entity.SetTableName(entity.GetTableName().ToSnakeCase());

            // Replace column names            
            foreach(var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName(StoreObjectIdentifier.Table(property.DeclaringEntityType.GetTableName(), null));
                property.SetColumnName(columnName.ToSnakeCase());
            }

            foreach(var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ToSnakeCase());
            }

            foreach(var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
            }

            foreach(var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
            }
        }
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}