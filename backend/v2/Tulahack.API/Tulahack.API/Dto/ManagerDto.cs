namespace Tulahack.API.Dto;

public partial class ManagerDto : PersonBaseDto
{
    public int Age { get; set; }
    public bool CertificateNeeded { get; set; }
    public int ManagerNumber { get; set; }
    public string CompanyName { get; set; }
    public string JobTitle { get; set; }
    public bool AttendingFirstTime { get; set; }
    public bool ApplicationConfirmationStatus { get; set; }
    public System.DateTime ApplicationSubmissionDate { get; set; }
    public string Region { get; set; }
    public string City { get; set; }
    public string University { get; set; }
    public string UniversityDepartment { get; set; }
    public string UniversityAttendanceFormat { get; set; }
    public string Scholarship { get; set; }
    public System.Guid ApplicationNumber { get; set; }
    public string AttendanceType { get; set; }
    public string EducationType { get; set; }
}