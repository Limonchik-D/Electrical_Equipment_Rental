using Electrical_Equipment_Rental.Data;
using Electrical_Equipment_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Controllers.Admin;

[Authorize(Roles = "Admin,Manager,Technician")]
[Route("admin/repairs")]
public class AdminRepairController : Controller
{
    private readonly AppDbContext _context;

    public AdminRepairController(AppDbContext context) => _context = context;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var repairs = await _context.Repairs
            .Include(r => r.ProductUnit).ThenInclude(u => u.Product)
            .Include(r => r.Technician)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return View(repairs);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var units = await _context.ProductUnits
            .Include(u => u.Product)
            .Select(u => new { u.Id, Label = u.SerialNumber + " — " + u.Product.Name })
            .ToListAsync();

        ViewBag.Units = new SelectList(units, "Id", "Label");
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(Repair model)
    {
        ModelState.Remove("ProductUnit");
        ModelState.Remove("Technician");

        var unit = await _context.ProductUnits.FindAsync(model.ProductUnitId);
        if (unit is not null) unit.Status = UnitStatus.Repair;
        model.CreatedAt = DateTime.UtcNow;

        _context.Repairs.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost("status/{id}")]
    public async Task<IActionResult> UpdateStatus(int id, RepairStatus status, decimal? cost)
    {
        var repair = await _context.Repairs
            .Include(r => r.ProductUnit)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (repair is null) return NotFound();

        repair.Status = status;
        if (cost.HasValue) repair.Cost = cost;

        if (status == RepairStatus.Done)
        {
            repair.CompletedAt = DateTime.UtcNow;
            repair.ProductUnit.Status = UnitStatus.Available;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
