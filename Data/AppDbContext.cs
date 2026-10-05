using BarcodeApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BarcodeApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarcodeApi.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Terminal> Terminals => Set<Terminal>();
        public DbSet<Scan> Scans => Set<Scan>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);   // required for the login tables

            b.Entity<Store>().HasIndex(s => s.Code).IsUnique();
            b.Entity<Terminal>().HasIndex(t => new { t.StoreId, t.Code }).IsUnique();
            b.Entity<Scan>().HasIndex(s => s.ClientScanId).IsUnique();
            b.Entity<Scan>().HasIndex(s => s.ScannedAtUtc);

            b.Entity<Scan>().HasOne(s => s.Store).WithMany()
                .HasForeignKey(s => s.StoreId).OnDelete(DeleteBehavior.Restrict);
            b.Entity<Scan>().HasOne(s => s.Terminal).WithMany()
                .HasForeignKey(s => s.TerminalId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
