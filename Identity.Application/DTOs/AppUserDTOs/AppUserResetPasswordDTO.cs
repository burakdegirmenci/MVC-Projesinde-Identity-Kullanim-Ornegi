﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.AppUserDTOs;

public class AppUserResetPasswordDTO
{
    public string Password { get; set; }
    public string Email { get; set; }
    public string Code { get; set; }
}
