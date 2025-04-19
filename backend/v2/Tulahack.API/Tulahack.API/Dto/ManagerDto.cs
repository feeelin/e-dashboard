namespace Tulahack.API.Dto;

public partial class ManagerDto : PersonBaseDto
{
    public int Age { get; set; }
    public bool CertificateNeeded { get; set; }
    public int ManagerNumber { get; set; }
    public string CompanyName { get; set; }
    public string JobTitle { get; set; }
}