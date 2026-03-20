namespace Electrical_Equipment_Rental.Models;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<ProductUnit> Units { get; set; } = new List<ProductUnit>();
    public ICollection<DeviceLocation> DeviceLocations { get; set; } = new List<DeviceLocation>();
}
