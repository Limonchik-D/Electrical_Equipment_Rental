namespace Electrical_Equipment_Rental.Models;

public class DeviceLocation
{
    public int Id { get; set; }
    public int ProductUnitId { get; set; }
    public ProductUnit ProductUnit { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public int? LocationId { get; set; }
    public Location? Location { get; set; }
}
