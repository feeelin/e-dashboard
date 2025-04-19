using Microsoft.EntityFrameworkCore;
using Tulahack.API.Models;

namespace Tulahack.API.Context;

public interface ITulahackContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<StorageFile> StorageFiles { get; set; }
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TimelineItem> TimelineItems { get; set; }

    public Task SaveChangesAsync();
    public Task SaveChangesAsync(CancellationToken cancellationToken);

    public Task<T> AddNewRecord<T>(T newItem) where T : class;
    public Task<T> UpdateRecord<T>(T record) where T : class;
    public Task<T> RemoveRecord<T>(T record) where T : class;
    public Task<T> AddNewRecord<T>(T newItem, CancellationToken cancellationToken) where T : class;
    public Task<T> UpdateRecord<T>(T record, CancellationToken cancellationToken) where T : class;
    public Task<T> RemoveRecord<T>(T record, CancellationToken cancellationToken) where T : class;
    public void ClearChangeTracker();
}