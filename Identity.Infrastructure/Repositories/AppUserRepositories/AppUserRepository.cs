using Identity.Domain.Entities;
using Identity.Infrastructure.Context;
using Identity.Infrastructure.DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Repositories.AppUserRepositories;

public class AppUserRepository : EFBaseRepository<AppUser>, IAppUserRepository
{
    public AppUserRepository(AppDbContext context) : base(context)
    {

    }

    public Task<AppUser?> GetByIdentityId(string identityId)
    {
        return _table.FirstOrDefaultAsync(x => x.IdentityId == identityId);
    }
}
