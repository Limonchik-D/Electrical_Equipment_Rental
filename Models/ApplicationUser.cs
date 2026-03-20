using Microsoft.AspNetCore.Identity;

namespace Electrical_Equipment_Rental.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsBlocked { get; set; } = false;
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
