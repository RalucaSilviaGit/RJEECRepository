using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public static class IdentityDataInitializer
    {
        public static void SeedData(UserManager<IdentityUser> userManager,
                                    RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (userManager.FindByEmailAsync("raluca.silvia87@yahoo.com").Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "raluca.silvia87@yahoo.com";
                user.Email = "raluca.silvia87@yahoo.com";
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "Network1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,"SuperAdmin").Wait();
                }
            }

            if (userManager.FindByEmailAsync("aralu10000@yahoo.com").Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "aralu10000@yahoo.com";
                user.Email = "aralu10000@yahoo.com";
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "Network1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,"Admin").Wait();
                }
            }

            if (userManager.FindByEmailAsync("reviewer@yahoo.com").Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "reviewer@yahoo.com";
                user.Email = "reviewer@yahoo.com";
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "Network1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Reviewer").Wait();
                }
            }

            if (userManager.FindByEmailAsync("editor@yahoo.com").Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "editor@yahoo.com";
                user.Email = "editor@yahoo.com";
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "Network1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Editor").Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("SuperAdmin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "SuperAdmin";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Editor").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Editor";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Reviewer").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Reviewer";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
