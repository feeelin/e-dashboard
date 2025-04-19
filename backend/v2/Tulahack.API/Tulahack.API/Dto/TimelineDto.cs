namespace Tulahack.API.Dto;

public enum TimelineItemTypeDto
{
    [System.Runtime.Serialization.EnumMember(Value = @"Unknown")]
    Unknown = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"Checkpoint")]
    Checkpoint = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"Deadline")]
    Deadline = 2,

    [System.Runtime.Serialization.EnumMember(Value = @"Event")]
    Event = 3,

    [System.Runtime.Serialization.EnumMember(Value = @"Meetup")]
    Meetup = 4,
}

public class TimelineDto
{
    public IEnumerable<TimelineItemDto> Items { get; set; }
    public DateTime HackathonStart { get; set; }
    public DateTime HackathonEnd { get; set; }
    public DateTime CodingBeginning { get; set; }
    public DateTime CodingDeadline { get; set; }
}

public class TimelineItemDto
{
    public TimelineItemTypeDto ItemType { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Label { get; set; }
    public DateTime? Deadline { get; set; }
    public string Url { get; set; }
    public string Message { get; set; }
    public string Extra { get; set; }
}