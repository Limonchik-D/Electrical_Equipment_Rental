namespace Electrical_Equipment_Rental.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal DepositAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<ProductUnit> Units { get; set; } = new List<ProductUnit>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
