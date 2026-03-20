namespace Electrical_Equipment_Rental.Models;

public enum RepairStatus
{
    Pending,
    InProgress,
    Done
}

public class Repair
{
    public int Id { get; set; }
    public int ProductUnitId { get; set; }
    public ProductUnit ProductUnit { get; set; } = null!;
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal? Cost { get; set; }
    public RepairStatus Status { get; set; } = RepairStatus.Pending;
    public string? TechnicianId { get; set; }
    public ApplicationUser? Technician { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
