using Electrical_Equipment_Rental.Data;
using Electrical_Equipment_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Controllers.Admin;

[Authorize(Roles = "Admin,Manager")]
[Route("admin/locations")]
public class AdminLocationController : Controller
{
    private readonly AppDbContext _context;

    public AdminLocationController(AppDbContext context) => _context = context;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var locations = await _context.Locations
            .Include(l => l.Units)
            .ToListAsync();
        return View(locations);
    }

    [HttpGet("create")]
    public IActionResult Create() => View();

    [HttpPost("create")]
    public async Task<IActionResult> Create(Location model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Locations.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location is null) return NotFound();
        return View(location);
    }

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, Location model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location is not null)
        {
            location.IsActive = false;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
