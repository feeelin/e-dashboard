namespace Tulahack.API.Models;

public enum ProjectStatus
{
    [System.Runtime.Serialization.EnumMember(Value = @"canceled")]
    Canceled = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"completed")]
    Completed = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"proposed")]
    Proposed = 2,

    [System.Runtime.Serialization.EnumMember(Value = @"inProgress")]
    InProgress = 3,

    [System.Runtime.Serialization.EnumMember(Value = @"rejected")]
    Rejected = 4,
    
    [System.Runtime.Serialization.EnumMember(Value = @"onHold")]
    OnHold = 5,
    
    [System.Runtime.Serialization.EnumMember(Value = @"blocked")]
    Blocked = 6,
}

public class Project
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public DateTime PlannedTime { get; set; }
    public DateTime ActualTime { get; set; }
    public DateTime AnalyticsTime { get; set; }
    public DateTime DevelopmentTime { get; set; }
    public DateTime TestingTime { get; set; }
    
    public ICollection<ProjectTask> ProjectTasks { get; set; }
    
    public string TimelineLabel { get; set; }
    public ICollection<TimelineItem> TimelineItems { get; set; }
    public ICollection<CapacityForecastItem> CapacityForecastItems { get; set; }
    
    public ProjectStatus Status { get; set; }
    
    // in hours
    public int TeamEfficiency { get; set; }
    
    // Task counter
    public int TeamWorkload { get; set; }

    // in hours
    public int TeamCapacity { get; set; }
    
    // in hours
    public int TaskLifecycle { get; set; }
    
    // just a number
    public int BugsAfterReleaseCounter { get; set; }
}