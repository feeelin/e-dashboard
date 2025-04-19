namespace Tulahack.API.Models;

public class Team
{
    public int Id { get; set; }

    public string Name { get; set; }
    
    public int MembersCount { get; set; }
    public int AssigneeWithOverdueTasks { get; set; }
    public int UnassignedMembers { get; set; }

}