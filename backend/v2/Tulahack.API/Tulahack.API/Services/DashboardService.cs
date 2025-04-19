using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tulahack.API.Context;
using Tulahack.API.Dto;
using Tulahack.API.Utils;

namespace Tulahack.API.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetOverview();
}

public class DashboardService : IDashboardService
{
    private readonly ITulahackContext _context;
    private readonly CdnConfiguration _cdnConfiguration;

    public DashboardService(
        ITulahackContext context,
        IOptions<CdnConfiguration> cdnConfiguration)
    {
        _context = context;
        _cdnConfiguration = cdnConfiguration.Value;
    }

    public async Task<DashboardDto> GetOverview()
    {
        var result = new DashboardDto
        {
            ParticipatorsCount = await _context.Accounts.CountAsync(),
            TopicThumbnailUrl = string.Concat(_cdnConfiguration.CdnUrl, "events/welcome.png"),
            UpcomingEventId = Guid.NewGuid(),
            Timeline = new TimelineDto
            {
                CodingBeginning = DateTime.Now.AddHours(2),
                CodingDeadline = DateTime.Now.AddHours(42),
                HackathonStart = DateTime.Now,
                HackathonEnd = DateTime.Now.AddHours(48),
                Items = new[]
                {
                    new TimelineItemDto
                    {
                        Start = DateTime.Now.AddHours(10),
                        Deadline = DateTime.Now.AddHours(11),
                        End = DateTime.Now.AddHours(12),
                        Label = "Checkpoint N1",
                        ItemType = TimelineItemTypeDto.Checkpoint,
                        Message = "Checkpoint is coming!",
                        Extra = "More",
                        Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
                    }
                }
            }
        };

        return result;
    }
}