namespace Tulahack.API.Models;

public partial class Manager : PersonBase
{
    public int Age { get; set; }
    public bool CertificateNeeded { get; set; }
    public int ManagerNumber { get; set; }
    public string CompanyName { get; set; }
    public string JobTitle { get; set; }
}