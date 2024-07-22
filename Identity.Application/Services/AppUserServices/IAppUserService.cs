using Identity.Application.DTOs.AppUserDTOs;
using Identity.Domain.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Services.AppUserServices;

public interface IAppUserService
{
    Task<IDataResult<List<AppUserListDTO>>> GetAllAsync();
    Task<IDataResult<AppUserDTO>> CreateAsync(AppUserCreateDTO appUserCreateDTO);
    Task<IResult> DeleteAsync(Guid id);
    Task<IDataResult<AppUserDTO>> GetByIdAsync(Guid id);
    Task<IDataResult<AppUserDTO>> UpdateAsync(AppUserUpdateDTO appUserUpdateDTO);
    //Change
    Task<IResult> ChangePasswordAsync(AppUserChangePasswordDTO appUserChangePasswordDTO);
    //Reset
    Task<IDataResult<AppUserForgotPasswordDTO>> ResetPasswordRequestAsync(string email);
    Task<IResult> ResetPasswordAsync(AppUserResetPasswordDTO adminResetPasswordDTO);

    Task<IDataResult<AppUserDTO>> GetByIdentityUserIdAsync(string identityUserId);
}
