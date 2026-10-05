using BarcodeApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarcodeApi.Data;

public static class SeedData
{
    public static async Task RunAsync(IServiceProvider services, IConfiguration config)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var users = services.GetRequiredService<UserManager<IdentityUser>>();

        if (!await db.Stores.AnyAsync())
        {
            var store = new Store { Code = "S001", Name = "Main Store" };
            store.Terminals.Add(new Terminal { Code = "T01", Name = "Counter 1" });
            store.Terminals.Add(new Terminal { Code = "T02", Name = "Counter 2" });
            db.Stores.Add(store);
            await db.SaveChangesAsync();
        }

        var email = config["Seed:UserEmail"];
        var password = config["Seed:UserPassword"];
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password)
            && await users.FindByEmailAsync(email) == null)
        {
            await users.CreateAsync(
                new IdentityUser { UserName = email, Email = email }, password);
        }
    }
}