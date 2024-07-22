using Identity.Infrastructure.Context;
using Identity.Infrastructure.Repositories.AppUserRepositories;
using Identity.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseLazyLoadingProxies();
            options.UseSqlServer(configuration.GetConnectionString(AppDbContext.DevConnectionString));
            
        });

        services.AddScoped<IAppUserRepository, AppUserRepository>();
        RoleSeed.SeedAsync(configuration).GetAwaiter().GetResult(); //Seed Data'daki verileri işler.

        return services;
    }
}
