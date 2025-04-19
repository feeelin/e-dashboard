namespace Tulahack.API.Models;

public enum TulahackRole
{
    [System.Runtime.Serialization.EnumMember(Value = @"Visitor")]
    Visitor = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"Manager")]
    Manager = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"Superuser")]
    Superuser = 2,
}
