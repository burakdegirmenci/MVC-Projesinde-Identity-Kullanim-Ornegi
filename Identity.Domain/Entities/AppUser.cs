using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities;

public class AppUser : BaseEntity
{

    public string Email { get; set; }
    public string Password { get; set; }
    public string IdentityId { get; set; }
}
