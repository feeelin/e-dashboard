namespace Tulahack.API.Models;

public partial class Manager : Account
{
    public bool CertificateNeeded { get; set; }
    public int ManagerNumber { get; set; }
}