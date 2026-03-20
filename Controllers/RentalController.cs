using Electrical_Equipment_Rental.Data;
using Electrical_Equipment_Rental.Models;
using Electrical_Equipment_Rental.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Controllers;

[Authorize]
public class RentalController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public RentalController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int unitId)
    {
        var unit = await _context.ProductUnits
            .Include(u => u.Product)
            .FirstOrDefaultAsync(u => u.Id == unitId && u.Status == UnitStatus.Available);

        if (unit is null) return NotFound();

        var vm = new RentalCreateViewModel
        {
            ProductUnitId = unit.Id,
            ProductName = unit.Product.Name,
            SerialNumber = unit.SerialNumber,
            PricePerHour = unit.Product.PricePerHour,
            DepositAmount = unit.Product.DepositAmount,
            Locations = await _context.Locations.Where(l => l.IsActive).ToListAsync()
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RentalCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Locations = await _context.Locations.Where(l => l.IsActive).ToListAsync();
            return View(vm);
        }

        var unit = await _context.ProductUnits
            .Include(u => u.Product)
            .FirstOrDefaultAsync(u => u.Id == vm.ProductUnitId && u.Status == UnitStatus.Available);

        if (unit is null)
        {
            ModelState.AddModelError(string.Empty, "Экземпляр уже недоступен");
            vm.Locations = await _context.Locations.Where(l => l.IsActive).ToListAsync();
            return View(vm);
        }

        var userId = _userManager.GetUserId(User)!;
        var rental = new Rental
        {
            UserId = userId,
            ProductUnitId = vm.ProductUnitId,
            PlannedEndAt = vm.PlannedEndAt.ToUniversalTime(),
            RatePerHour = unit.Product.PricePerHour,
            DepositAmount = unit.Product.DepositAmount,
            StartLocationId = vm.StartLocationId,
            Notes = vm.Notes,
            Status = RentalStatus.Pending
        };

        unit.Status = UnitStatus.Rented;
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Аренда оформлена! Ожидайте подтверждения менеджера.";
        return RedirectToAction("MyRentals");
    }

    public async Task<IActionResult> MyRentals()
    {
        var userId = _userManager.GetUserId(User)!;
        var rentals = await _context.Rentals
            .Include(r => r.ProductUnit).ThenInclude(u => u.Product)
            .Include(r => r.StartLocation)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(rentals);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var rental = await _context.Rentals
            .Include(r => r.ProductUnit)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId && r.Status == RentalStatus.Pending);

        if (rental is null) return NotFound();

        rental.Status = RentalStatus.Cancelled;
        rental.ProductUnit.Status = UnitStatus.Available;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Аренда отменена.";
        return RedirectToAction("MyRentals");
    }
}
