using Microsoft.AspNetCore.Identity;

namespace CarParkingSystem.Areas.Identity.Data
{
    public static class ContextSeed
    {

        public static async Task seedRolesAsync(RoleManager<IdentityRole> roleManager)
        {

            string[] roleNames = { "Admin", "Guard", "Student" };

            foreach (var roleName in roleNames)
            {
                // Check if the role already exists
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Create the role if it doesn't exist
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser defaultAdmin = new ApplicationUser
            {
                UserName = "87654321@Org4life.ac.za",
                Email = "87654321@Org4life.ac.za",
                EmailConfirmed = true,
                FirstName = "Web site",
                LastName = "Owner",
                idNumber = 1234567891234,
                orgNum = 87654321,
                IsAccountActive = true,
            };
            if (userManager.Users.All(u => u.Id != defaultAdmin.Id))
            {
                var user = await userManager.CreateAsync(defaultAdmin, "SuperAdmin123@");

                if (user.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultAdmin, "Admin");

                }
            }

        }

    }
}
