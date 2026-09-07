using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;
using Microsoft.AspNetCore.Identity;


namespace LabConsumableExpiryTracker.Data.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedRolesAndSuperAdminAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            // 1. Seed Roles
            string[] roles = [UserRole.WarehouseAdmin.ToString(),  UserRole.Scientist.ToString()];
            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            // 2. Seed WarehousAdmin
            var warehouseAdminUsername = "warehouseadmin";
            var existingAdmin = await userManager.FindByNameAsync(warehouseAdminUsername);

            if (existingAdmin == null)
            {
                var warehouseAdmin = new User(warehouseAdminUsername, "admin@lab.com");

                var result = await userManager.CreateAsync(warehouseAdmin, "WarehouseAdmin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(warehouseAdmin, UserRole.WarehouseAdmin.ToString());
                }
            }
        }
    }
}