using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Tulahack.API.Models;

namespace Tulahack.API.Controllers;

[ApiController]
[Route("api/mock/[controller]")]
public class MockedContestController : ControllerBase
{
    private readonly Fixture _fixture;
    private readonly ILogger<MockedContestController> _logger;

    public MockedContestController(ILogger<MockedContestController> logger)
    {
        _logger = logger;
        _fixture = new Fixture();

        // --- AutoFixture Customizations ---

        // 1. Handle potential circular references in models (e.g., Project <-> Tasks)
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // 2. Customize creation for specific models if needed (optional)
        _fixture.Customize<Project>(composer => composer
            .Without(p => p.Id) // Let Id be assigned sequentially or by request below
            .With(p => p.Status, () => _fixture.Create<ProjectStatus>()) // Ensure enum gets generated
            // Auto-populate collections (adjust counts as needed)
            .With(p => p.ProjectTasks, () => _fixture.CreateMany<ProjectTask>(30).ToList())
            .With(p => p.TimelineItems, () => _fixture.CreateMany<TimelineItem>(30).ToList())
        );

        _fixture.Customize<ProjectTask>(composer => composer
                .Without(pt => pt.Id) // Let Id be assigned sequentially or by request below
                .With(pt => pt.TaskType, () => _fixture.Create<ProjectTaskType>())
                .With(pt => pt.TaskLabel, () => _fixture.Create<ProjectTaskLabel>())
                .With(pt => pt.Priority, () => _fixture.Create<ProjectTaskPriority>())
                .With(pt => pt.EstimationAccuracy, () => _fixture.Create<ProjectTaskEstimationAccuracy>())
                // Let ActualTime be generated based on its type (ProjectTaskStatus enum)
                .With(pt => pt.ActualTime, () => _fixture.Create<ProjectTaskStatus>())
            // IMPORTANT: Clear ProjectId or set it contextually in methods below
            // .Without(pt => pt.ProjectId) // Avoids conflicts if ProjectTask is generated standalone
        );

        _fixture.Customize<TimelineItem>(composer => composer
                .Without(ti => ti.Id)
                .With(ti => ti.ItemType, () => _fixture.Create<TimelineItemType>())
            // .Without(ti => ti.ProjectId)
        );

        _fixture.Customize<Team>(composer => composer.Without(t => t.Id));
    }

    // Helper to simulate relationship linking
    private void LinkProjectData(Project project)
    {
        // Ensure ProjectTasks have the correct ProjectId
        if (project.ProjectTasks != null)
        {
            foreach (var task in project.ProjectTasks)
            {
                // Assuming ProjectTask has ProjectId (as per migration snapshot)
                task.GetType().GetProperty("ProjectId")?.SetValue(task, project.Id);
                // AutoFixture might have already generated an ID, let's give it a new one maybe?
                // task.Id = _fixture.Create<int>(); // Or handle IDs better
            }
        }

        // Ensure TimelineItems have the correct ProjectId
        if (project.TimelineItems != null)
        {
            foreach (var item in project.TimelineItems)
            {
                item.GetType().GetProperty("ProjectId")?.SetValue(item, project.Id);
                // item.Id = _fixture.Create<int>();
            }
        }

        // Ensure CapcityForecastItems have the correct ProjectId
        if (project.TimelineItems != null)
        {
            foreach (var item in project.TimelineItems)
            {
                item.GetType().GetProperty("ProjectId")?.SetValue(item, project.Id);
                // item.Id = _fixture.Create<int>();
            }
        }
    }

    [HttpGet("projects")]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetAllProjects(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Getting all projects");
        var projects = _fixture.CreateMany<Project>(7).ToList();
        // Assign sequential IDs and link data AFTER generation
        for (int i = 0; i < projects.Count; i++)
        {
            projects[i].Id = i + 1; // Simple sequential IDs for mock
            LinkProjectData(projects[i]);
        }

        return Task.FromResult<IActionResult>(Ok(projects));
    }

    [HttpGet("projects/{projectId:int}")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetProjectById(int projectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Getting project by ID: {ProjectId}", projectId);
        // Simulate not found occasionally
        if (projectId % 10 == 9)
        {
            // e.g., IDs ending in 9 are not found
            _logger.LogWarning("MOCK: Project ID {ProjectId} simulated as NotFound", projectId);
            return Task.FromResult<IActionResult>(NotFound());
        }

        var project = _fixture.Build<Project>()
            .With(p => p.Id, projectId)
            .Create();
        LinkProjectData(project); // Link related data
        return Task.FromResult<IActionResult>(Ok(project));
    }

    [HttpPost("projects")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateProject([FromBody] Project project, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Creating project: {ProjectName}", project?.Name);
        if (project == null) return Task.FromResult<IActionResult>(BadRequest("Project data is null"));

        // Simulate assigning a new ID and returning the 'created' object
        var createdProject = _fixture.Build<Project>()
            .With(p => p.Id, _fixture.Create<int>()) // Generate a new ID
            .With(p => p.Name, project.Name) // Use name from input
            .Create();
        LinkProjectData(createdProject);

        _logger.LogInformation("MOCK: Project created with ID: {ProjectId}", createdProject.Id);
        return Task.FromResult<IActionResult>(CreatedAtAction(nameof(GetProjectById),
            new { projectId = createdProject.Id }, createdProject));
    }

    [HttpPut("projects/{projectId:int}")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateProject(int projectId, [FromBody] Project project,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Updating project ID: {ProjectId}", projectId);
        if (project == null || projectId != project.Id)
        {
            _logger.LogWarning("MOCK: Update failed - ID mismatch or null body.");
            return Task.FromResult<IActionResult>(BadRequest("ID mismatch or missing body."));
        }

        // Simulate not found occasionally
        if (projectId % 10 == 9)
        {
            _logger.LogWarning("MOCK: Project ID {ProjectId} simulated as NotFound for update", projectId);
            return Task.FromResult<IActionResult>(NotFound());
        }

        // Return a new mocked object representing the updated state
        var updatedProject = _fixture.Build<Project>()
            .With(p => p.Id, projectId) // Keep the original ID
            .With(p => p.Name, project.Name) // Reflect updated name
            // Other properties will be random from AutoFixture
            .Create();
        LinkProjectData(updatedProject);
        _logger.LogInformation("MOCK: Project updated for ID: {ProjectId}", updatedProject.Id);
        return Task.FromResult<IActionResult>(Ok(updatedProject));
    }

    [HttpDelete("projects/{projectId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteProject(int projectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Deleting project ID: {ProjectId}", projectId);
        // Simulate not found occasionally
        if (projectId % 10 == 9)
        {
            _logger.LogWarning("MOCK: Project ID {ProjectId} simulated as NotFound for delete", projectId);
            return Task.FromResult<IActionResult>(NotFound());
        }

        _logger.LogInformation("MOCK: Project deleted for ID: {ProjectId}", projectId);
        return Task.FromResult<IActionResult>(NoContent());
    }

    [HttpGet("tasks")]
    [ProducesResponseType(typeof(IEnumerable<ProjectTask>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetAllProjectTasks([FromQuery] int? projectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Getting tasks {ProjectIdFilter}",
            projectId.HasValue ? $"for project ID: {projectId}" : "(all)");
        int count = 100;
        var tasks = _fixture.CreateMany<ProjectTask>(count).ToList();

        // If projectId is specified, filter or assign the ProjectId
        if (projectId.HasValue)
        {
            tasks = _fixture.Build<ProjectTask>()
                // .With(pt => pt.ProjectId, projectId.Value) // Set the ProjectId correctly
                .Do(pt => pt.GetType().GetProperty("ProjectId")?.SetValue(pt, projectId.Value)) // Set FK property
                .CreateMany<ProjectTask>(count / 2) // Generate fewer if filtered
                .ToList();
        }

        // Assign sequential IDs
        for (int i = 0; i < tasks.Count; i++)
        {
            tasks[i].Id = i + 100;
        } // Start IDs higher

        return Task.FromResult<IActionResult>(Ok(tasks));
    }

    [HttpGet("tasks/{taskId:int}")]
    [ProducesResponseType(typeof(ProjectTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetProjectTaskById(int taskId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Getting task by ID: {TaskId}", taskId);
        if (taskId % 10 == 9) return Task.FromResult<IActionResult>(NotFound());

        var task = _fixture.Build<ProjectTask>()
            .With(pt => pt.Id, taskId)
            // Optionally associate with a mock project ID
            .Do(pt => pt.GetType().GetProperty("ProjectId")
                ?.SetValue(pt, _fixture.Create<int>() % 10)) // Random project ID 0-9
            .Create();
        return Task.FromResult<IActionResult>(Ok(task));
    }

    [HttpPost("tasks")]
    [ProducesResponseType(typeof(ProjectTask), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateProjectTask([FromBody] ProjectTask task, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Creating task: {TaskName}", task?.Name);
        if (task == null) return Task.FromResult<IActionResult>(BadRequest("Task data is null"));

        var createdTask = _fixture.Build<ProjectTask>()
            .With(pt => pt.Id, _fixture.Create<int>()) // New ID
            .With(pt => pt.Name, task.Name) // Use input name
            // Use ProjectId from input if provided
            .Do(pt => pt.GetType().GetProperty("ProjectId")
                ?.SetValue(pt, task.GetType().GetProperty("ProjectId")?.GetValue(task)))
            .Create();

        _logger.LogInformation("MOCK: Task created with ID: {TaskId}", createdTask.Id);
        return Task.FromResult<IActionResult>(CreatedAtAction(nameof(GetProjectTaskById),
            new { taskId = createdTask.Id }, createdTask));
    }

    [HttpPut("tasks/{taskId:int}")]
    [ProducesResponseType(typeof(ProjectTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateProjectTask(int taskId, [FromBody] ProjectTask task,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Updating task ID: {TaskId}", taskId);
        if (task == null || taskId != task.Id)
            return Task.FromResult<IActionResult>(BadRequest("ID mismatch or missing body."));
        if (taskId % 10 == 9) return Task.FromResult<IActionResult>(NotFound());

        var updatedTask = _fixture.Build<ProjectTask>()
            .With(pt => pt.Id, taskId) // Keep ID
            .With(pt => pt.Name, task.Name) // Use input name
            .Do(pt => pt.GetType().GetProperty("ProjectId")
                ?.SetValue(pt, task.GetType().GetProperty("ProjectId")?.GetValue(task)))
            .Create();
        _logger.LogInformation("MOCK: Task updated for ID: {TaskId}", updatedTask.Id);
        return Task.FromResult<IActionResult>(Ok(updatedTask));
    }

    [HttpDelete("tasks/{taskId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteProjectTask(int taskId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Deleting task ID: {TaskId}", taskId);
        if (taskId % 10 == 9) return Task.FromResult<IActionResult>(NotFound());
        _logger.LogInformation("MOCK: Task deleted for ID: {TaskId}", taskId);
        return Task.FromResult<IActionResult>(NoContent());
    }

    [HttpGet("teams")]
    [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetAllTeams(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Getting all teams");
        var teams = _fixture.CreateMany<Team>(8).ToList();
        for (int i = 0; i < teams.Count; i++)
        {
            teams[i].Id = i + 1;
        }

        return Task.FromResult<IActionResult>(Ok(teams));
    }

    [HttpGet("teams/{teamId:int}")]
    [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetTeamById(int teamId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Getting team by ID: {TeamId}", teamId);
        if (teamId > 10) return Task.FromResult<IActionResult>(NotFound()); // Simulate not found for high IDs

        var team = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .Create();
        return Task.FromResult<IActionResult>(Ok(team));
    }

    [HttpPost("teams")]
    [ProducesResponseType(typeof(Team), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateTeam([FromBody] Team team, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Creating team: {TeamName}", team?.Name);
        if (team == null) return Task.FromResult<IActionResult>(BadRequest("Team data is null"));

        var createdTeam = _fixture.Build<Team>()
            .With(t => t.Id, _fixture.Create<int>() % 50 + 10) // Generate ID > 10
            .With(t => t.Name, team.Name)
            .Create();

        _logger.LogInformation("MOCK: Team created with ID: {TeamId}", createdTeam.Id);
        return Task.FromResult<IActionResult>(CreatedAtAction(nameof(GetTeamById), new { teamId = createdTeam.Id },
            createdTeam));
    }

    [HttpPut("teams/{teamId:int}")]
    [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateTeam(int teamId, [FromBody] Team team, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Updating team ID: {TeamId}", teamId);
        if (team == null || teamId != team.Id)
            return Task.FromResult<IActionResult>(BadRequest("ID mismatch or missing body."));
        if (teamId > 10) return Task.FromResult<IActionResult>(NotFound());

        var updatedTeam = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .With(t => t.Name, team.Name)
            .Create();
        _logger.LogInformation("MOCK: Team updated for ID: {TeamId}", updatedTeam.Id);
        return Task.FromResult<IActionResult>(Ok(updatedTeam));
    }

    [HttpDelete("teams/{teamId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteTeam(int teamId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Deleting team ID: {TeamId}", teamId);
        if (teamId > 10) return Task.FromResult<IActionResult>(NotFound());
        _logger.LogInformation("MOCK: Team deleted for ID: {TeamId}", teamId);
        return Task.FromResult<IActionResult>(NoContent());
    }

    [HttpGet("timeline-items")]
    [ProducesResponseType(typeof(IEnumerable<TimelineItem>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetAllTimelineItems([FromQuery] int? projectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Getting timeline items {ProjectIdFilter}",
            projectId.HasValue ? $"for project ID: {projectId}" : "(all)");
        int count = 50;
        var items = _fixture.CreateMany<TimelineItem>(count).ToList();

        if (projectId.HasValue)
        {
            items = _fixture.Build<TimelineItem>()
                .Do(ti => ti.GetType().GetProperty("ProjectId")?.SetValue(ti, projectId.Value))
                .CreateMany<TimelineItem>(count / 2)
                .ToList();
        }

        // Assign sequential IDs
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Id = i + 500;
        } // Start IDs higher

        return Task.FromResult<IActionResult>(Ok(items));
    }

    [HttpGet("timeline-items/{timelineItemId:int}")]
    [ProducesResponseType(typeof(TimelineItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetTimelineItemById(int timelineItemId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Getting timeline item by ID: {TimelineItemId}", timelineItemId);
        if (timelineItemId % 10 == 9) return Task.FromResult<IActionResult>(NotFound());

        var item = _fixture.Build<TimelineItem>()
            .With(ti => ti.Id, timelineItemId)
            .Do(ti => ti.GetType().GetProperty("ProjectId")
                ?.SetValue(ti, _fixture.Create<int>() % 10)) // Random project ID 0-9
            .CreateMany(50);
        return Task.FromResult<IActionResult>(Ok(item));
    }

    [HttpPost("timeline-items")]
    [ProducesResponseType(typeof(TimelineItem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateTimelineItem([FromBody] TimelineItem timelineItem,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Creating timeline item: {Message}", timelineItem?.Message);
        if (timelineItem == null) return Task.FromResult<IActionResult>(BadRequest("Item data is null"));

        var createdItem = _fixture.Build<TimelineItem>()
            .With(ti => ti.Id, _fixture.Create<int>()) // New ID
            .With(ti => ti.Message, timelineItem.Message) // Use input message
            .Do(ti => ti.GetType().GetProperty("ProjectId")?.SetValue(ti,
                timelineItem.GetType().GetProperty("ProjectId")?.GetValue(timelineItem)))
            .Create();

        _logger.LogInformation("MOCK: Timeline item created with ID: {TimelineItemId}", createdItem.Id);
        return Task.FromResult<IActionResult>(CreatedAtAction(nameof(GetTimelineItemById),
            new { timelineItemId = createdItem.Id }, createdItem));
    }

    [HttpPut("timeline-items/{timelineItemId:int}")]
    [ProducesResponseType(typeof(TimelineItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateTimelineItem(int timelineItemId, [FromBody] TimelineItem timelineItem,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Updating timeline item ID: {TimelineItemId}", timelineItemId);
        if (timelineItem == null || timelineItemId != timelineItem.Id)
            return Task.FromResult<IActionResult>(BadRequest("ID mismatch or missing body."));
        if (timelineItemId % 10 == 9) return Task.FromResult<IActionResult>(NotFound());

        var updatedItem = _fixture.Build<TimelineItem>()
            .With(ti => ti.Id, timelineItemId) // Keep ID
            .With(ti => ti.Message, timelineItem.Message) // Use input message
            .Do(ti => ti.GetType().GetProperty("ProjectId")?.SetValue(ti,
                timelineItem.GetType().GetProperty("ProjectId")?.GetValue(timelineItem)))
            .Create();
        _logger.LogInformation("MOCK: Timeline item updated for ID: {TimelineItemId}", updatedItem.Id);
        return Task.FromResult<IActionResult>(Ok(updatedItem));
    }

    [HttpDelete("timeline-items/{timelineItemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteTimelineItem(int timelineItemId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Deleting timeline item ID: {TimelineItemId}", timelineItemId);
        if (timelineItemId % 10 == 9) return Task.FromResult<IActionResult>(NotFound());
        _logger.LogInformation("MOCK: Timeline item deleted for ID: {TimelineItemId}", timelineItemId);
        return Task.FromResult<IActionResult>(NoContent());
    }
}