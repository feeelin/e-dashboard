namespace Tulahack.API.Models;

public enum FilePurposeType
{
    [System.Runtime.Serialization.EnumMember(Value = @"Unknown")]
    Unknown = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"Default")]
    Default = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"TaskExtras")]
    TaskExtras = 2

}

public partial class StorageFile
{
    public Guid Id { get; set; }

    public string Filename { get; set; }

    public string Filepath { get; set; }

    public Guid Owner { get; set; }

    public DateTime CreationDate { get; set; }

    public int Revision { get; set; }

    public FilePurposeType Purpose { get; set; }
}