using Microsoft.AspNetCore.Identity;

namespace EventManagmentTask.Data
{
    public class SeedingRoles
    {
        public static async Task SeedingRolesAsync (RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Client", "Organizer" };

            foreach(var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
