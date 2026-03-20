namespace Electrical_Equipment_Rental.Models;

public enum PaymentType
{
    Deposit,
    Rental,
    Fine,
    Refund
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Refunded
}

public class Payment
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public Rental Rental { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentType Type { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
}
