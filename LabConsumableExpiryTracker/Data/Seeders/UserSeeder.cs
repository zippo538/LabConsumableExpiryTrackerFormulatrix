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
            string[] roles = [UserRole.WareHouseAdmin.ToString(),  UserRole.Scientist.ToString()];
            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            // 2. Seed SuperAdmin
            var superAdminUsername = "warehouseadmin";
            var existingAdmin = await userManager.FindByNameAsync(superAdminUsername);

            if (existingAdmin == null)
            {
                var superAdmin = new User(superAdminUsername, "admin@lab.com");

                var result = await userManager.CreateAsync(superAdmin, "WarehouseAdmin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdmin, UserRole.WareHouseAdmin.ToString());
                }
            }
        }
    }
}