namespace Tulahack.API.Dto;

public class PersonBaseDto
{
    public Guid Id { get; set; }
    public string Firstname { get; set; }
    public string Middlename { get; set; }
    public string Lastname { get; set; }
    public string TelegramAccount { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string About { get; set; }
    public string PhotoUrl { get; set; }
    public bool Blocked { get; set; }
}