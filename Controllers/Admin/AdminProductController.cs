using Electrical_Equipment_Rental.Data;
using Electrical_Equipment_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Controllers.Admin;

[Authorize(Roles = "Admin,Manager")]
[Route("admin/products")]
public class AdminProductController : Controller
{
    private readonly AppDbContext _context;

    public AdminProductController(AppDbContext context) => _context = context;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Units)
            .OrderBy(p => p.CategoryId)
            .ToListAsync();
        return View(products);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(Product model)
    {
        ModelState.Remove("Category");
        ModelState.Remove("Units");
        ModelState.Remove("Images");

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(model);
        }

        _context.Products.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, Product model)
    {
        if (id != model.Id) return BadRequest();

        ModelState.Remove("Category");
        ModelState.Remove("Units");
        ModelState.Remove("Images");

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet("units/{productId}")]
    public async Task<IActionResult> Units(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Units).ThenInclude(u => u.CurrentLocation)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null) return NotFound();

        ViewBag.Locations = new SelectList(await _context.Locations.Where(l => l.IsActive).ToListAsync(), "Id", "Name");
        return View(product);
    }

    [HttpPost("units/{productId}/add")]
    public async Task<IActionResult> AddUnit(int productId, string serialNumber, int? locationId)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            TempData["Error"] = "Серийный номер обязателен";
            return RedirectToAction("Units", new { productId });
        }

        _context.ProductUnits.Add(new ProductUnit
        {
            ProductId = productId,
            SerialNumber = serialNumber,
            CurrentLocationId = locationId,
            Status = UnitStatus.Available
        });
        await _context.SaveChangesAsync();
        return RedirectToAction("Units", new { productId });
    }

    [HttpPost("units/status")]
    public async Task<IActionResult> ChangeUnitStatus(int unitId, UnitStatus status, int productId)
    {
        var unit = await _context.ProductUnits.FindAsync(unitId);
        if (unit is not null)
        {
            unit.Status = status;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Units", new { productId });
    }
}
