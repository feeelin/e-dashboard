namespace Tulahack.API.Models;

public enum TimelineItemType
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

public class TimelineItem
{
    public int Id { get; set; }

    public TimelineItemType ItemType { get; set; }
    public string Message { get; set; }
    public string Extra { get; set; }
}