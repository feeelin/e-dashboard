using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tulahack.API.Models;

namespace Tulahack.API.Context;

public class TulahackContext : DbContext, ITulahackContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<StorageFile> StorageFiles { get; set; }
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TimelineItem> TimelineItems { get; set; }

    Task ITulahackContext.SaveChangesAsync() => SaveChangesAsync();
    Task ITulahackContext.SaveChangesAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

    public TulahackContext(DbContextOptions<TulahackContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // how to create tables with model inheritance and entity type hierarchy mapping
        // https://learn.microsoft.com/en-us/ef/core/modeling/inheritance

        // Table-per-concrete-type configuration
        // Separate table for each entity: Accounts, Contestants, Experts and Moderators
        // modelBuilder.Entity<PersonBase>()
        //     .UseTpcMappingStrategy()
        //     .ToTable("Accounts")
        //     .Property(p => p.Id)
        //     .ValueGeneratedOnAdd();
        // modelBuilder.Entity<Contestant>()
        //     .ToTable("Contestants")
        //     .Property(p => p.Id)
        //     .ValueGeneratedOnAdd();
        // modelBuilder.Entity<Expert>()
        //     .ToTable("Experts")
        //     .Property(p => p.Id)
        //     .ValueGeneratedOnAdd();
        // modelBuilder.Entity<Moderator>()
        //     .ToTable("Moderators")
        //     .Property(p => p.Id)
        //     .ValueGeneratedOnAdd();
        // modelBuilder.Entity<Team>()
        //     .ToTable("Teams")
        //     .Property(p => p.Id)
        //     .ValueGeneratedOnAdd();

        // Table-per-hierarchy and discriminator configuration
        // Single table 'Accounts' for PersonBase, Contestant, Expert and Moderator entities
        _ = modelBuilder.Entity<Account>()
            .HasDiscriminator(item => item.Role)
            .HasValue<Account>(TulahackRole.Visitor)
            .HasValue<Manager>(TulahackRole.Manager)
            .HasValue<Superuser>(TulahackRole.Superuser);
        _ = modelBuilder.Entity<Account>()
            .ToTable("Accounts")
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
        _ = modelBuilder.Entity<Manager>();
        _ = modelBuilder.Entity<Superuser>();

        // ugly hack to enable inherited model type mutation
        modelBuilder.Entity<Account>()
            .Property<TulahackRole>(nameof(Account.Role))
            .Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Save);
        _ = modelBuilder.Entity<StorageFile>()
            .ToTable("StorageFiles")
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }

    public async Task<T> AddNewRecord<T>(T newItem) where T : class
    {
        DbSet<T> dbSet = Set<T>();
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> result = dbSet.Add(newItem);
        _ = await SaveChangesAsync();
        return result.Entity;
    }

    public async Task<T> UpdateRecord<T>(T record) where T : class
    {
        DbSet<T> dbSet = Set<T>();
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> result = dbSet.Update(record);
        _ = await SaveChangesAsync();
        return result.Entity;
    }

    public async Task<T> RemoveRecord<T>(T record) where T : class
    {
        DbSet<T> dbSet = Set<T>();
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> result = dbSet.Remove(record);
        _ = await SaveChangesAsync();
        return result.Entity;
    }

    public async Task<T> AddNewRecord<T>(T newItem, CancellationToken cancellationToken) where T : class
    {
        DbSet<T> dbSet = Set<T>();
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> result = dbSet.Add(newItem);
        _ = await SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<T> UpdateRecord<T>(T record, CancellationToken cancellationToken) where T : class
    {
        DbSet<T> dbSet = Set<T>();
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> result = dbSet.Update(record);
        _ = await SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<T> RemoveRecord<T>(T record, CancellationToken cancellationToken) where T : class
    {
        DbSet<T> dbSet = Set<T>();
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> result = dbSet.Remove(record);
        _ = await SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public void ClearChangeTracker() => this.ChangeTracker.Clear();
}