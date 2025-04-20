using Microsoft.EntityFrameworkCore;
using Tulahack.API.Context;
using Tulahack.API.Models;

namespace Tulahack.API.Services;

public interface IContestService
{
    Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken cancellationToken = default);
    Task<Project?> GetProjectByIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Project> CreateProjectAsync(Project project, CancellationToken cancellationToken = default);
    Task<Project?> UpdateProjectAsync(int projectId, Project project, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectAsync(int projectId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectTask>> GetAllProjectTasksAsync(int? projectId = null, CancellationToken cancellationToken = default);
    Task<ProjectTask?> GetProjectTaskByIdAsync(int taskId, CancellationToken cancellationToken = default);
    Task<ProjectTask> CreateProjectTaskAsync(ProjectTask task, CancellationToken cancellationToken = default);
    Task<ProjectTask?> UpdateProjectTaskAsync(int taskId, ProjectTask task, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectTaskAsync(int taskId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Team>> GetAllTeamsAsync(CancellationToken cancellationToken = default);
    Task<Team?> GetTeamByIdAsync(int teamId, CancellationToken cancellationToken = default);
    Task<Team> CreateTeamAsync(Team team, CancellationToken cancellationToken = default);
    Task<Team?> UpdateTeamAsync(int teamId, Team team, CancellationToken cancellationToken = default);
    Task<bool> DeleteTeamAsync(int teamId, CancellationToken cancellationToken = default);

    Task<IEnumerable<TimelineItem>> GetAllTimelineItemsAsync(int? projectId = null, CancellationToken cancellationToken = default);
    Task<TimelineItem?> GetTimelineItemByIdAsync(int timelineItemId, CancellationToken cancellationToken = default);
    Task<List<TimelineItem>> GetTimelineItemsByProjectIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TimelineItem> CreateTimelineItemAsync(TimelineItem timelineItem, CancellationToken cancellationToken = default);
    Task<TimelineItem?> UpdateTimelineItemAsync(int timelineItemId, TimelineItem timelineItem, CancellationToken cancellationToken = default);
    Task<bool> DeleteTimelineItemAsync(int timelineItemId, CancellationToken cancellationToken = default);
}

public class ContestService : IContestService
{
    private readonly ITulahackContext _context;
    private readonly ILogger<ContestService> _logger;

    public ContestService(ITulahackContext context, ILogger<ContestService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Project CRUD
    public async Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all projects");
        return await _context.Projects
                             .Include(p => p.ProjectTasks) // Optional: Include related data
                             .Include(p => p.TimelineItems) // Optional: Include related data
                             .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetProjectByIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching project with ID: {ProjectId}", projectId);
        return await _context.Projects
                             .Include(p => p.ProjectTasks) // Optional: Include related data
                             .Include(p => p.TimelineItems) // Optional: Include related data
                             .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
    }

    public async Task<Project> CreateProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new project: {ProjectName}", project.Name);
        // Optionally add validation or default values here
        return await _context.AddNewRecord(project, cancellationToken);
    }

    public async Task<Project?> UpdateProjectAsync(int projectId, Project project, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating project with ID: {ProjectId}", projectId);
        if (projectId != project.Id)
        {
            _logger.LogWarning("Mismatch between route ID ({RouteId}) and project ID ({ProjectId})", projectId, project.Id);
            return null; // Or throw BadRequestException
        }

        var existingProject = await _context.Projects.FindAsync(new object[] { projectId }, cancellationToken);
        if (existingProject == null)
        {
            _logger.LogWarning("Project with ID: {ProjectId} not found for update", projectId);
            return null;
        }

        // Use the context's generic update method which should handle tracking
        // Note: This replaces *all* scalar properties. Navigation properties need explicit handling if needed.
        _context.ClearChangeTracker(); // Ensure clean tracking state if necessary
        return await _context.UpdateRecord(project, cancellationToken);

        /* Alternative manual update (more control):
        _context.Entry(existingProject).CurrentValues.SetValues(project);
        // Handle updates to collections like ProjectTasks or TimelineItems if necessary
        await _context.SaveChangesAsync(cancellationToken);
        return existingProject;
        */
    }

    public async Task<bool> DeleteProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete project with ID: {ProjectId}", projectId);
        var projectToDelete = await _context.Projects.FindAsync(new object[] { projectId }, cancellationToken);
        if (projectToDelete == null)
        {
            _logger.LogWarning("Project with ID: {ProjectId} not found for deletion", projectId);
            return false;
        }

        await _context.RemoveRecord(projectToDelete, cancellationToken);
        return true;
    }
    #endregion

    #region ProjectTask CRUD
    public async Task<IEnumerable<ProjectTask>> GetAllProjectTasksAsync(int? projectId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all project tasks {ProjectIdFilter}", projectId.HasValue ? $"for project ID: {projectId}" : "");
        var query = _context.ProjectTasks.AsQueryable();
        if (projectId.HasValue)
        {
            // Assuming ProjectTask has a ProjectId foreign key property (add it if missing)
             query = query.Where(pt => EF.Property<int?>(pt, "ProjectId") == projectId.Value);
            // If ProjectTask has a direct navigation property Project, you could use:
            // query = query.Where(pt => pt.Project.Id == projectId.Value);
        }
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ProjectTask?> GetProjectTaskByIdAsync(int taskId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching project task with ID: {TaskId}", taskId);
        return await _context.ProjectTasks.FindAsync(new object[] { taskId }, cancellationToken);
    }

    public async Task<ProjectTask> CreateProjectTaskAsync(ProjectTask task, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new project task: {TaskName}", task.Name);
        return await _context.AddNewRecord(task, cancellationToken);
    }

    public async Task<ProjectTask?> UpdateProjectTaskAsync(int taskId, ProjectTask task, CancellationToken cancellationToken = default)
    {
         _logger.LogInformation("Updating project task with ID: {TaskId}", taskId);
        if (taskId != task.Id)
        {
            _logger.LogWarning("Mismatch between route ID ({RouteId}) and task ID ({TaskId})", taskId, task.Id);
            return null;
        }

        var existingTask = await _context.ProjectTasks.FindAsync(new object[] { taskId }, cancellationToken);
        if (existingTask == null)
        {
            _logger.LogWarning("Project task with ID: {TaskId} not found for update", taskId);
            return null;
        }

        _context.ClearChangeTracker();
        return await _context.UpdateRecord(task, cancellationToken);
    }

    public async Task<bool> DeleteProjectTaskAsync(int taskId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete project task with ID: {TaskId}", taskId);
        var taskToDelete = await _context.ProjectTasks.FindAsync(new object[] { taskId }, cancellationToken);
        if (taskToDelete == null)
        {
            _logger.LogWarning("Project task with ID: {TaskId} not found for deletion", taskId);
            return false;
        }

        await _context.RemoveRecord(taskToDelete, cancellationToken);
        return true;
    }
    #endregion

    #region Team CRUD
    public async Task<IEnumerable<Team>> GetAllTeamsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all teams");
        return await _context.Teams.ToListAsync(cancellationToken);
    }

    public async Task<Team?> GetTeamByIdAsync(int teamId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching team with ID: {TeamId}", teamId);
        return await _context.Teams.FindAsync(new object[] { teamId }, cancellationToken);
    }

    public async Task<Team> CreateTeamAsync(Team team, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new team: {TeamName}", team.Name);
        return await _context.AddNewRecord(team, cancellationToken);
    }

    public async Task<Team?> UpdateTeamAsync(int teamId, Team team, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating team with ID: {TeamId}", teamId);
         if (teamId != team.Id)
        {
            _logger.LogWarning("Mismatch between route ID ({RouteId}) and team ID ({TeamId})", teamId, team.Id);
            return null;
        }
        var existingTeam = await _context.Teams.FindAsync(new object[] { teamId }, cancellationToken);
        if (existingTeam == null)
        {
            _logger.LogWarning("Team with ID: {TeamId} not found for update", teamId);
            return null;
        }
        _context.ClearChangeTracker();
        return await _context.UpdateRecord(team, cancellationToken);
    }

    public async Task<bool> DeleteTeamAsync(int teamId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete team with ID: {TeamId}", teamId);
        var teamToDelete = await _context.Teams.FindAsync(new object[] { teamId }, cancellationToken);
        if (teamToDelete == null)
        {
            _logger.LogWarning("Team with ID: {TeamId} not found for deletion", teamId);
            return false;
        }
        await _context.RemoveRecord(teamToDelete, cancellationToken);
        return true;
    }
    #endregion

    #region TimelineItem CRUD
    public async Task<IEnumerable<TimelineItem>> GetAllTimelineItemsAsync(int? projectId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all timeline items {ProjectIdFilter}", projectId.HasValue ? $"for project ID: {projectId}" : "");
        var query = _context.TimelineItems.AsQueryable();
        if (projectId.HasValue)
        {
             // Assuming TimelineItem has a ProjectId foreign key property (add it if missing)
             query = query.Where(ti => EF.Property<int?>(ti, "ProjectId") == projectId.Value);
        }
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TimelineItem?> GetTimelineItemByIdAsync(int timelineItemId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching timeline item with ID: {TimelineItemId}", timelineItemId);
        return await _context.TimelineItems.FindAsync(new object[] { timelineItemId }, cancellationToken);
    }

        public async Task<List<TimelineItem>> GetTimelineItemsByProjectIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching timelines item with ID: {Id}", id);
        return await _context.TimelineItems.ToListAsync(cancellationToken);
    }
    
    public async Task<TimelineItem> CreateTimelineItemAsync(TimelineItem timelineItem, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new timeline item: {TimelineItemMessage}", timelineItem.Message);
        return await _context.AddNewRecord(timelineItem, cancellationToken);
    }

    public async Task<TimelineItem?> UpdateTimelineItemAsync(int timelineItemId, TimelineItem timelineItem, CancellationToken cancellationToken = default)
    {
         _logger.LogInformation("Updating timeline item with ID: {TimelineItemId}", timelineItemId);
        if (timelineItemId != timelineItem.Id)
        {
            _logger.LogWarning("Mismatch between route ID ({RouteId}) and timeline item ID ({TimelineItemId})", timelineItemId, timelineItem.Id);
            return null;
        }
        var existingItem = await _context.TimelineItems.FindAsync(new object[] { timelineItemId }, cancellationToken);
        if (existingItem == null)
        {
            _logger.LogWarning("Timeline item with ID: {TimelineItemId} not found for update", timelineItemId);
            return null;
        }
        _context.ClearChangeTracker();
        return await _context.UpdateRecord(timelineItem, cancellationToken);
    }

    public async Task<bool> DeleteTimelineItemAsync(int timelineItemId, CancellationToken cancellationToken = default)
    {
         _logger.LogInformation("Attempting to delete timeline item with ID: {TimelineItemId}", timelineItemId);
        var itemToDelete = await _context.TimelineItems.FindAsync(new object[] { timelineItemId }, cancellationToken);
        if (itemToDelete == null)
        {
             _logger.LogWarning("Timeline item with ID: {TimelineItemId} not found for deletion", timelineItemId);
            return false;
        }
        await _context.RemoveRecord(itemToDelete, cancellationToken);
        return true;
    }
    #endregion
}