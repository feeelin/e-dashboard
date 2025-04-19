namespace Tulahack.API.Dto;

public partial class DashboardDto
{
    public int ParticipatorsCount { get; set; }
    public int ExpertsCount { get; set; }
    public int CompaniesCount { get; set; }
    public int CasesCount { get; set; }
    public int TeamsCount { get; set; }
    public string TopicTitle { get; set; }
    public string Topic { get; set; }
    public string TopicThumbnailUrl { get; set; }
    public System.Guid UpcomingEventId { get; set; }
    public string EventThumbnailUrl { get; set; }
    public TimelineDto Timeline { get; set; }
}