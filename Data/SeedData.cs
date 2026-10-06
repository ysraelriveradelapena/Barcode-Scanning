using BarcodeApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarcodeApi.Data;

public static class SeedData
{
    public static async Task RunAsync(IServiceProvider services, IConfiguration config)
    {
        try
        {
            var db = services.GetRequiredService<AppDbContext>();
            var users = services.GetRequiredService<UserManager<IdentityUser>>();

            var email = config["Seed:UserEmail"];
            var password = config["Seed:UserPassword"];

            if (!await db.Stores.AnyAsync())
            {
                var store = new Store { Code = "S001", Name = "Main Store" };
                store.Terminals.Add(new Terminal { Code = "T01", Name = "Counter 1" });
                store.Terminals.Add(new Terminal { Code = "T02", Name = "Counter 2" });

                db.Stores.Add(store);
                await db.SaveChangesAsync();
            }

            var existingUser = await users.FindByEmailAsync(email);

            if (existingUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email
                };

                var result = await users.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("SEED ERROR: " + ex.Message);
        }
    }
}