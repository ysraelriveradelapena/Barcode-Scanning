using BarcodeApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarcodeApi.Controllers;

public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var scans = await _db.Scans
            .OrderByDescending(x => x.ScannedAtUtc)
            .Take(100)
            .ToListAsync();

        ViewBag.TotalScans = await _db.Scans.CountAsync();

        return View(scans);
    }
}