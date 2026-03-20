using Electrical_Equipment_Rental.Data;
using Electrical_Equipment_Rental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Controllers;

public class CatalogController : Controller
{
    private readonly AppDbContext _context;

    public CatalogController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index(int? categoryId, string? search)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Units)
            .Include(p => p.Images)
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));

        ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
        ViewBag.SelectedCategory = categoryId;
        ViewBag.Search = search;

        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Units.Where(u => u.Status == UnitStatus.Available))
                .ThenInclude(u => u.CurrentLocation)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product is null) return NotFound();

        return View(product);
    }
}
