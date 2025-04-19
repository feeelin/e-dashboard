namespace Tulahack.API.Static;

public static class TulahackClaimTypes
{
    public static string Group = "group";
    public static string KeycloakGroup = "http://schemas.xmlsoap.org/claims/Group";
}

public static class Groups
{
    public static readonly string Public = "/Public";
    public static readonly string Managers = "/Managers";
    public static readonly string Expert = "/Experts";
    public static readonly string Moderator = "/Moderators";
    public static readonly string Superuser = "/Superusers";

    public static List<string> PublicPlus = new() { Public, Managers, Expert, Moderator, Superuser };
    public static List<string> ManagerPlus = new() { Managers, Expert, Moderator, Superuser };
    public static List<string> ExpertPlus = new() { Expert, Moderator, Superuser };
    public static List<string> ModeratorPlus = new() { Moderator, Superuser };
}