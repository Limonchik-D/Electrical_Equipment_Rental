using Electrical_Equipment_Rental.Data;
using Electrical_Equipment_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Controllers.Admin;

[Authorize(Roles = "Admin,Manager")]
[Route("admin/rentals")]
public class AdminRentalController : Controller
{
    private readonly AppDbContext _context;

    public AdminRentalController(AppDbContext context) => _context = context;

    [HttpGet("")]
    public async Task<IActionResult> Index(RentalStatus? status)
    {
        var query = _context.Rentals
            .Include(r => r.User)
            .Include(r => r.ProductUnit).ThenInclude(u => u.Product)
            .Include(r => r.StartLocation)
            .AsQueryable();

        if (status.HasValue) query = query.Where(r => r.Status == status);

        ViewBag.Status = status;
        return View(await query.OrderByDescending(r => r.CreatedAt).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var rental = await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.ProductUnit).ThenInclude(u => u.Product)
            .Include(r => r.StartLocation)
            .Include(r => r.EndLocation)
            .Include(r => r.Payments)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rental is null) return NotFound();

        ViewBag.Locations = await _context.Locations.Where(l => l.IsActive).ToListAsync();
        return View(rental);
    }

    [HttpPost("activate/{id}")]
    public async Task<IActionResult> Activate(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental is null || rental.Status != RentalStatus.Pending) return NotFound();

        rental.Status = RentalStatus.Active;
        rental.StartedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id });
    }

    [HttpPost("complete/{id}")]
    public async Task<IActionResult> Complete(int id, int? endLocationId)
    {
        var rental = await _context.Rentals
            .Include(r => r.ProductUnit)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rental is null || rental.Status != RentalStatus.Active) return NotFound();

        rental.ActualEndAt = DateTime.UtcNow;
        rental.Status = RentalStatus.Completed;
        rental.EndLocationId = endLocationId;

        if (rental.StartedAt.HasValue)
        {
            var hours = (decimal)(rental.ActualEndAt.Value - rental.StartedAt.Value).TotalHours;
            rental.TotalAmount = Math.Round(hours * rental.RatePerHour, 2);

            if (rental.PlannedEndAt.HasValue && rental.ActualEndAt > rental.PlannedEndAt)
            {
                var overtimeHours = (decimal)(rental.ActualEndAt.Value - rental.PlannedEndAt.Value).TotalHours;
                rental.FineAmount = Math.Round(overtimeHours * rental.RatePerHour * 1.5m, 2);
            }
        }

        rental.ProductUnit.Status = UnitStatus.Available;
        rental.ProductUnit.CurrentLocationId = endLocationId;
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id });
    }

    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var rental = await _context.Rentals
            .Include(r => r.ProductUnit)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rental is null) return NotFound();

        rental.Status = RentalStatus.Cancelled;
        if (rental.ProductUnit.Status == UnitStatus.Rented)
            rental.ProductUnit.Status = UnitStatus.Available;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
