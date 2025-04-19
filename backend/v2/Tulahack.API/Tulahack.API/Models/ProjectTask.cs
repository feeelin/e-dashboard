namespace Tulahack.API.Models;

public enum ProjectTaskType
{
    Bug,
    Regular
}

public enum ProjectTaskPriority
{
    Critical,
    High,
    Normal,
    Low
}

public enum ProjectTaskEstimationAccuracy
{
    CompletedEarly,
    CompletedLate,
    CompletedInTime
}

public enum ProjectTaskLabel
{
    Production,
    Stage
}

public enum ProjectTaskStatus
{
    ToDo,
    InProgress,
    Completed
}

public class ProjectTask
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Assignee { get; set; }

    // In hours
    public int Estimated { get; set; }

    // In hours
    public int Actual { get; set; }

    // In hours
    public int TimeTracked { get; set; }

    public ProjectTaskStatus ActualTime { get; set; }
    public ProjectTaskType TaskType { get; set; }
    public ProjectTaskLabel TaskLabel { get; set; }

    public ProjectTaskPriority Priority { get; set; }
    public ProjectTaskEstimationAccuracy EstimationAccuracy { get; set; }
}