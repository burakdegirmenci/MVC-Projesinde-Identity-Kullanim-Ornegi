using Identity.Domain.Entities;
using Identity.Infrastructure.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Repositories.AppUserRepositories;

public interface IAppUserRepository : IAsyncRepository, IAsyncInsertableRepository<AppUser>, IAsyncFindableRepository<AppUser>, IAsyncQueryableRepository<AppUser>, IAsyncUpdatableRepository<AppUser>, IAsyncDeletableRepository<AppUser>, IAsyncTransactionRepository
{
    Task<AppUser?> GetByIdentityId(string identityId);
}
