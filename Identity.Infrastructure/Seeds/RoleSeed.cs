using Identity.Domain.Enums;
using Identity.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Seeds
{
    internal class RoleSeed
    {
        public static async Task SeedAsync(IConfiguration configuration)
        {
            var dbContextBuilder = new DbContextOptionsBuilder<AppDbContext>();
            dbContextBuilder.UseSqlServer(configuration.GetConnectionString("AppConnectionDev"));
            AppDbContext context = new AppDbContext(dbContextBuilder.Options);
            if (!context.Roles.Any(role => role.Name == "AppUser"))
            {
                await AddRoles(context);
            }
        }
        private static async Task AddRoles(AppDbContext context)
        {

            string[] roles = Enum.GetNames(typeof(Role));  // Enumda bulunan verilerin isimlerini dizi olarak döner
            for (int i = 0; i < roles.Length; i++)
            {
                //Enum'dan rolleri alır ve eğer veritabanında o rol yoksa ekler.
                if (await context.Roles.AnyAsync(role => role.Name == roles[i]))
                {
                    continue;
                }
                IdentityRole role = new IdentityRole()
                {
                    Name = roles[i],
                    NormalizedName = roles[i].ToUpperInvariant()
                };
                await context.Roles.AddAsync(role);
                await context.SaveChangesAsync();
            }
        }
    }
}
