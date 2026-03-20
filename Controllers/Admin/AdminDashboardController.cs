using Electrical_Equipment_Rental.Data;
using Electrical_Equipment_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Controllers.Admin;

[Authorize(Roles = "Admin,Manager")]
[Route("admin")]
public class AdminDashboardController : Controller
{
    private readonly AppDbContext _context;

    public AdminDashboardController(AppDbContext context) => _context = context;

    [HttpGet("")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> Index()
    {
        ViewBag.UsersCount = await _context.Users.CountAsync();
        ViewBag.ProductsCount = await _context.Products.CountAsync();
        ViewBag.UnitsCount = await _context.ProductUnits.CountAsync();
        ViewBag.ActiveRentals = await _context.Rentals.CountAsync(r => r.Status == RentalStatus.Active);
        ViewBag.PendingRentals = await _context.Rentals.CountAsync(r => r.Status == RentalStatus.Pending);
        ViewBag.ActiveRepairs = await _context.Repairs.CountAsync(r => r.Status != RepairStatus.Done);
        ViewBag.AvailableUnits = await _context.ProductUnits.CountAsync(u => u.Status == UnitStatus.Available);
        ViewBag.RecentRentals = await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.ProductUnit).ThenInclude(u => u.Product)
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .ToListAsync();

        return View();
    }
}
