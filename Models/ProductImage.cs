namespace Electrical_Equipment_Rental.Models;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; } = false;
}
