using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BarcodeApi.Data;
using BarcodeApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarcodeApi.Controllers;

[ApiController]
[Route("api/scans")]
[Authorize]   // no valid token = 401
public class ScansController : ControllerBase
{
    private readonly AppDbContext _db;
    public ScansController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Post(ScanRequest req)
    {
        var code = req.Code.Trim();
        if (code.Length == 0 || code.Length > 100)
            return BadRequest("Invalid barcode.");
        if (req.ClientScanId == Guid.Empty)
            return BadRequest("ClientScanId is required.");

        // Same scan sent twice? Return success without saving again.
        var existing = await _db.Scans
            .FirstOrDefaultAsync(s => s.ClientScanId == req.ClientScanId);
        if (existing != null)
            return Ok(new { id = existing.Id, duplicate = true });

        // The terminal must exist AND belong to the given store.
        var terminal = await _db.Terminals.Include(t => t.Store)
            .FirstOrDefaultAsync(t => t.Code == req.TerminalCode
                                   && t.Store!.Code == req.StoreCode);
        if (terminal == null)
            return BadRequest("Unknown store or terminal.");

        var scan = new Scan
        {
            ClientScanId = req.ClientScanId,
            Code = code,
            Format = req.Format,
            ScannedAtUtc = req.ScannedAt.ToUniversalTime(),
            UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)!,
            StoreId = terminal.StoreId,
            TerminalId = terminal.Id
        };

        _db.Scans.Add(scan);
        await _db.SaveChangesAsync();
        return Ok(new { id = scan.Id, duplicate = false });
    }

    [HttpGet("recent")]
    public async Task<IActionResult> Recent()
    {
        var rows = await _db.Scans
            .OrderByDescending(s => s.ScannedAtUtc)
            .Take(20)
            .Select(s => new
            {
                s.Id,
                s.Code,
                s.Format,
                s.ScannedAtUtc,
                Store = s.Store!.Name,
                Terminal = s.Terminal!.Name,
                User = s.User!.Email
            })
            .ToListAsync();

        return Ok(rows);
    }
}