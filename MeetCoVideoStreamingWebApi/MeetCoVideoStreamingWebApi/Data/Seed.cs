using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MeetCoVideoStreamingWebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<Users> userManager, RoleManager<Roles> roleManager)
        {
            try
            {
                if (await userManager.Users.AnyAsync())
                {
                    return;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
           

            var roles = new List<Roles>
            {
                new Roles{Id = Guid.NewGuid(), Name = "SuperAdmin"},
                new Roles{Id = Guid.NewGuid(), Name = "Admin"}
            };

            if (!await roleManager.Roles.AnyAsync())
            {
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }

            var users = new List<Users> {
                new Users { UserName = "AdeelK", DisplayName = "Adeel Khan" },
                new Users{ UserName="Gomm", DisplayName = "Robert Gomm" }
            };

            foreach (var user in users)
            {
                //user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "admin@123");
                await userManager.AddToRoleAsync(user, "Proctor");
            }

            var admin = new Users { UserName = "admin", DisplayName = "Administrator" };
            await userManager.CreateAsync(admin, "admin@123");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Proctor", "Meetinginee" });
        }
    }
}
