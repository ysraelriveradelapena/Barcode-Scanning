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

    [AllowAnonymous]
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("alive");

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post(ScanRequest req)
    {
        try
        {
            var code = req.Code.Trim();

            if (code.Length == 0 || code.Length > 100)
                return BadRequest("Invalid barcode.");

            if (req.ClientScanId == Guid.Empty)
                return BadRequest("ClientScanId is required.");

            var existing = await _db.Scans
                .FirstOrDefaultAsync(s => s.ClientScanId == req.ClientScanId);

            if (existing != null)
                return Ok(new { id = existing.Id, duplicate = true });

            var terminal = await _db.Terminals
                .Include(t => t.Store)
                .FirstOrDefaultAsync(t =>
                    t.Code == req.TerminalCode &&
                    t.Store!.Code == req.StoreCode);

            if (terminal == null)
                return BadRequest("Unknown store or terminal.");

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token - no user");

            var scan = new Scan
            {
                ClientScanId = req.ClientScanId,
                Code = code,
                Format = req.Format,
                ScannedAtUtc = DateTime.UtcNow,
                UserId = userId,
                StoreId = terminal.StoreId,
                TerminalId = terminal.Id
            };

            _db.Scans.Add(scan);
            await _db.SaveChangesAsync();

            return Ok(new { id = scan.Id, duplicate = false });
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Scan API crashed",
                detail: ex.InnerException?.Message ?? ex.Message
            );
        }
    }

    [HttpGet("recent")]
    [Authorize]
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
                s.StoreId,
                s.TerminalId,
                s.UserId
            })
            .ToListAsync();

        return Ok(rows);
    }
}