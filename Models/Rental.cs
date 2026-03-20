namespace Electrical_Equipment_Rental.Models;

public enum RentalStatus
{
    Pending,
    Active,
    Completed,
    Cancelled
}

public class Rental
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int ProductUnitId { get; set; }
    public ProductUnit ProductUnit { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? PlannedEndAt { get; set; }
    public DateTime? ActualEndAt { get; set; }
    public decimal RatePerHour { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? FineAmount { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Pending;
    public int StartLocationId { get; set; }
    public Location StartLocation { get; set; } = null!;
    public int? EndLocationId { get; set; }
    public Location? EndLocation { get; set; }
    public string? Notes { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
