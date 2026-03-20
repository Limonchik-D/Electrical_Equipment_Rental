namespace Electrical_Equipment_Rental.Models;

public enum UnitStatus
{
    Available,
    Rented,
    Repair,
    Retired
}

public class ProductUnit
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public UnitStatus Status { get; set; } = UnitStatus.Available;
    public int? CurrentLocationId { get; set; }
    public Location? CurrentLocation { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    public ICollection<DeviceLocation> LocationHistory { get; set; } = new List<DeviceLocation>();
}
