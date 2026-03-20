using Electrical_Equipment_Rental.Data;
using Electrical_Equipment_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/users")]
public class AdminUserController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUserController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
        return View(users);
    }

    [HttpPost("block/{id}")]
    public async Task<IActionResult> Block(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is not null)
        {
            user.IsBlocked = !user.IsBlocked;
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction("Index");
    }
}
