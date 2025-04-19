namespace Tulahack.API.Models;

public class CapacityForecastItem
{
    public string ProjectName { get; set; }
    
    public DateTime Date { get; set; }
    
    // in hours
    public int Value { get; set; }
    
    public bool IsPredicted { get; set; }
}