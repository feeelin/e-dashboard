using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tulahack.API.Models;
using Tulahack.API.Services;

namespace Tulahack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// Apply appropriate authorization - adjust policies as needed
// [Authorize(Policy = "Public+")] // Example: Allow reads for many
public class ContestController : ControllerBase
{
    private readonly IContestService _contestService;
    private readonly ILogger<ContestController> _logger;

    public ContestController(IContestService contestService, ILogger<ContestController> logger)
    {
        _contestService = contestService;
        _logger = logger;
    }

    [HttpGet("projects")]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    [Authorize(Policy = "Public+")] // Allow broader access for reads
    public async Task<IActionResult> GetAllProjects(CancellationToken cancellationToken)
    {
        var projects = await _contestService.GetAllProjectsAsync(cancellationToken);
        return Ok(projects);
    }

    [HttpGet("projects/{projectId:int}")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Public+")] // Allow broader access for reads
    public async Task<IActionResult> GetProjectById(int projectId, CancellationToken cancellationToken)
    {
        var project = await _contestService.GetProjectByIdAsync(projectId, cancellationToken);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPost("projects")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "Manager")] // Example: Restrict creation
    public async Task<IActionResult> CreateProject([FromBody] Project project, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProject = await _contestService.CreateProjectAsync(project, cancellationToken);
        // Return 201 Created with the location of the new resource
        return CreatedAtAction(nameof(GetProjectById), new { projectId = createdProject.Id }, createdProject);
    }

    [HttpPut("projects/{projectId:int}")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Manager")] // Example: Restrict update
    public async Task<IActionResult> UpdateProject(int projectId, [FromBody] Project project,
        CancellationToken cancellationToken)
    {
        if (projectId != project.Id)
        {
            ModelState.AddModelError("Id", "Route ID does not match project ID in body.");
            return BadRequest(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedProject = await _contestService.UpdateProjectAsync(projectId, project, cancellationToken);
        return updatedProject == null ? NotFound() : Ok(updatedProject);
    }

    [HttpDelete("projects/{projectId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Superuser")] // Example: Restrict deletion further
    public async Task<IActionResult> DeleteProject(int projectId, CancellationToken cancellationToken)
    {
        var success = await _contestService.DeleteProjectAsync(projectId, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("tasks")]
    [ProducesResponseType(typeof(IEnumerable<ProjectTask>), StatusCodes.Status200OK)]
    [Authorize(Policy = "Public+")]
    public async Task<IActionResult> GetAllProjectTasks([FromQuery] int? projectId, CancellationToken cancellationToken)
    {
        var tasks = await _contestService.GetAllProjectTasksAsync(projectId, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("tasks/{taskId:int}")]
    [ProducesResponseType(typeof(ProjectTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Public+")]
    public async Task<IActionResult> GetProjectTaskById(int taskId, CancellationToken cancellationToken)
    {
        var task = await _contestService.GetProjectTaskByIdAsync(taskId, cancellationToken);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpPost("tasks")]
    [ProducesResponseType(typeof(ProjectTask), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> CreateProjectTask([FromBody] ProjectTask task, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Consider validating that the related Project exists if ProjectId is provided
        var createdTask = await _contestService.CreateProjectTaskAsync(task, cancellationToken);
        return CreatedAtAction(nameof(GetProjectTaskById), new { taskId = createdTask.Id }, createdTask);
    }

    [HttpPut("tasks/{taskId:int}")]
    [ProducesResponseType(typeof(ProjectTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> UpdateProjectTask(int taskId, [FromBody] ProjectTask task,
        CancellationToken cancellationToken)
    {
        if (taskId != task.Id)
        {
            ModelState.AddModelError("Id", "Route ID does not match task ID in body.");
            return BadRequest(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedTask = await _contestService.UpdateProjectTaskAsync(taskId, task, cancellationToken);
        return updatedTask == null ? NotFound() : Ok(updatedTask);
    }

    [HttpDelete("tasks/{taskId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Manager")] // Or Superuser
    public async Task<IActionResult> DeleteProjectTask(int taskId, CancellationToken cancellationToken)
    {
        var success = await _contestService.DeleteProjectTaskAsync(taskId, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("teams")]
    [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
    [Authorize(Policy = "Public+")]
    public async Task<IActionResult> GetAllTeams(CancellationToken cancellationToken)
    {
        var teams = await _contestService.GetAllTeamsAsync(cancellationToken);
        return Ok(teams);
    }

    [HttpGet("teams/{teamId:int}")]
    [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Public+")]
    public async Task<IActionResult> GetTeamById(int teamId, CancellationToken cancellationToken)
    {
        var team = await _contestService.GetTeamByIdAsync(teamId, cancellationToken);
        return team == null ? NotFound() : Ok(team);
    }

    [HttpPost("teams")]
    [ProducesResponseType(typeof(Team), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "Superuser")] // Example: Only Superusers can create teams
    public async Task<IActionResult> CreateTeam([FromBody] Team team, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdTeam = await _contestService.CreateTeamAsync(team, cancellationToken);
        return CreatedAtAction(nameof(GetTeamById), new { teamId = createdTeam.Id }, createdTeam);
    }

    [HttpPut("teams/{teamId:int}")]
    [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Superuser")] // Example: Only Superusers can update teams
    public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] Team team, CancellationToken cancellationToken)
    {
        if (teamId != team.Id)
        {
            ModelState.AddModelError("Id", "Route ID does not match team ID in body.");
            return BadRequest(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedTeam = await _contestService.UpdateTeamAsync(teamId, team, cancellationToken);
        return updatedTeam == null ? NotFound() : Ok(updatedTeam);
    }

    [HttpDelete("teams/{teamId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Superuser")] // Example: Only Superusers can delete teams
    public async Task<IActionResult> DeleteTeam(int teamId, CancellationToken cancellationToken)
    {
        var success = await _contestService.DeleteTeamAsync(teamId, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("timeline-items")]
    [ProducesResponseType(typeof(IEnumerable<TimelineItem>), StatusCodes.Status200OK)]
    [Authorize(Policy = "Public+")]
    public async Task<IActionResult> GetAllTimelineItems([FromQuery] int? projectId,
        CancellationToken cancellationToken)
    {
        var items = await _contestService.GetAllTimelineItemsAsync(projectId, cancellationToken);
        return Ok(items);
    }

    [HttpGet("timeline-items/single/{timelineItemId:int}")]
    [ProducesResponseType(typeof(TimelineItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Public+")]
    public async Task<IActionResult> GetTimelineItemById(int timelineItemId, CancellationToken cancellationToken)
    {
        var item = await _contestService.GetTimelineItemByIdAsync(timelineItemId, cancellationToken);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("timeline-items/{projectId:int}")]
    [ProducesResponseType(typeof(TimelineItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Public+")]
    public async Task<IActionResult> GetTimelineItemByProjectId(int projectId, CancellationToken cancellationToken)
    {
        var item = await _contestService.GetTimelineItemsByProjectIdAsync(projectId, cancellationToken);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("timeline-items")]
    [ProducesResponseType(typeof(TimelineItem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "Manager")] // Example: Managers can add timeline items
    public async Task<IActionResult> CreateTimelineItem([FromBody] TimelineItem timelineItem,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Consider validating that the related Project exists if ProjectId is provided
        var createdItem = await _contestService.CreateTimelineItemAsync(timelineItem, cancellationToken);
        return CreatedAtAction(nameof(GetTimelineItemById), new { timelineItemId = createdItem.Id }, createdItem);
    }

    [HttpPut("timeline-items/{timelineItemId:int}")]
    [ProducesResponseType(typeof(TimelineItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Manager")] // Example: Managers can update timeline items
    public async Task<IActionResult> UpdateTimelineItem(int timelineItemId, [FromBody] TimelineItem timelineItem,
        CancellationToken cancellationToken)
    {
        if (timelineItemId != timelineItem.Id)
        {
            ModelState.AddModelError("Id", "Route ID does not match timeline item ID in body.");
            return BadRequest(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedItem =
            await _contestService.UpdateTimelineItemAsync(timelineItemId, timelineItem, cancellationToken);
        return updatedItem == null ? NotFound() : Ok(updatedItem);
    }

    [HttpDelete("timeline-items/{timelineItemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Superuser")] // Example: Only Superusers can delete timeline items
    public async Task<IActionResult> DeleteTimelineItem(int timelineItemId, CancellationToken cancellationToken)
    {
        var success = await _contestService.DeleteTimelineItemAsync(timelineItemId, cancellationToken);
        return success ? NoContent() : NotFound();
    }
}