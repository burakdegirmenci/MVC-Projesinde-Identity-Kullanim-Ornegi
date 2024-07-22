using Identity.Application.DTOs.AppUserDTOs;
using Identity.Application.Services.UserServices;
using Identity.Domain.Entities;
using Identity.Domain.Utilities.Concretes;
using Identity.Domain.Utilities.Interfaces;
using Identity.Infrastructure.Repositories.AppUserRepositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Services.AppUserServices;

public class AppUserService : IAppUserService
{
    private readonly IAppUserRepository _appUserRepository;
    private readonly IUserService _userService;

    public AppUserService(IAppUserRepository appUserRepository, IUserService userService)
    {
        _appUserRepository = appUserRepository;
        _userService = userService;
    }

    public async Task<IResult> ChangePasswordAsync(AppUserChangePasswordDTO appUserChangePasswordDTO)
    {
        var appUser = await _appUserRepository.GetByIdAsync(appUserChangePasswordDTO.Id);
        if (appUser == null)
        {
            return new ErrorResult("AppUser could not be found!");
        }

        var identityUser = await _userService.FindByIdAsync(appUser.IdentityId);
        if (identityUser == null)
        {
            return new ErrorResult("Identity user could not be found!");
        }

        var result = await _userService.ChangePasswordAsync(identityUser, appUserChangePasswordDTO.CurrentPassword, appUserChangePasswordDTO.NewPassword);
        if (!result.Succeeded)
        {
            return new ErrorResult("Password could not be changed!");
        }

        return new SuccessResult("Password could change successfuly!");
    }

    public async Task<IDataResult<AppUserDTO>> CreateAsync(AppUserCreateDTO appUserCreateDTO)
    {
        if (await _userService.AnyAsync(x => x.Email == appUserCreateDTO.Email))
        {
            return new ErrorDataResult<AppUserDTO>("Email is already taken!");
        }
        IdentityUser identityUser = new()
        {
            Email = appUserCreateDTO.Email,
            NormalizedEmail = appUserCreateDTO.Email.ToUpperInvariant(),
            UserName = appUserCreateDTO.Email,
            NormalizedUserName = appUserCreateDTO.Email.ToUpperInvariant(),
            EmailConfirmed = true
        };

        DataResult<AppUserDTO> result = new ErrorDataResult<AppUserDTO>();

        var strategy = await _appUserRepository.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            var transactionScope = await _appUserRepository.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var identityResult = await _userService.CreateUserAsync(identityUser, appUserCreateDTO.Password, Domain.Enums.Role.AppUser);
                if (!identityResult.Succeeded)
                {
                    result = new ErrorDataResult<AppUserDTO>(identityResult.ToString());
                    transactionScope.Rollback();
                    return;
                }
                var appUsereUser = appUserCreateDTO.Adapt<AppUser>();
                appUsereUser.IdentityId = identityUser.Id;
                await _appUserRepository.AddAsync(appUsereUser);
                await _appUserRepository.SaveChangesAsync();
                result = new SuccessDataResult<AppUserDTO>("AppUser added successfully!");
                transactionScope.Commit();
            }
            catch (Exception ex)
            {
                result = new ErrorDataResult<AppUserDTO>("AppUser could not be added!" + ex.Message);
                transactionScope.Rollback();

            }
            finally
            {
                transactionScope.Dispose();
            }
        });
        return result;
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var deletingAppUser = await _appUserRepository.GetByIdAsync(id);
        if (deletingAppUser == null)
        {
            return new ErrorDataResult<IResult>("AppUser could not be found!");
        }

        var strategy = await _appUserRepository.CreateExecutionStrategy();
        Result result = new ErrorResult();

        await strategy.ExecuteAsync(async () =>
        {
            var transactionScope = await _appUserRepository.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var identityResult = await _userService.DeleteUserAsync(deletingAppUser.IdentityId);
                if (!identityResult.Succeeded)
                {
                    result = new ErrorResult(identityResult.ToString());
                    transactionScope.Rollback();
                    return;
                }
                await _appUserRepository.DeleteAsync(deletingAppUser);
                await _appUserRepository.SaveChangesAsync();
                result = new SuccessResult("AppUser deleted successfully!");
                transactionScope.Commit();
            }
            catch (Exception ex)
            {
                result = new ErrorResult("AppUser could not be deleted" + ex.Message);
                transactionScope.Rollback();
            }
            finally
            {
                transactionScope.Dispose();
            }
        });
        return result;
    }

    public async Task<IDataResult<List<AppUserListDTO>>> GetAllAsync()
    {
        var appUsers = await _appUserRepository.GetAllAsync();
        var appUserDTOs = appUsers.Adapt<List<AppUserListDTO>>();
        if (appUsers.Count() <= 0)
        {
            return new ErrorDataResult<List<AppUserListDTO>>(appUserDTOs, "AppUser could not be found!");
        }
        return new SuccessDataResult<List<AppUserListDTO>>(appUserDTOs, "AppUser listed successfully!");
    }

    public async Task<IDataResult<AppUserDTO>> GetByIdAsync(Guid id)
    {
        var appUser = await _appUserRepository.GetByIdAsync(id);
        if (appUser == null)
        {
            return new ErrorDataResult<AppUserDTO>("AppUser could not be found!");
        }
        var appUserDTO = appUser.Adapt<AppUserDTO>();
        return new SuccessDataResult<AppUserDTO>(appUserDTO, "AppUser found successfully!");
    }

    public async Task<IDataResult<AppUserDTO>> GetByIdentityUserIdAsync(string identityUserId)
    {
        var appUser = await _appUserRepository.GetByIdentityId(identityUserId);
        if (appUser == null)
        {
            return new ErrorDataResult<AppUserDTO>("AppUser could not be found!");
        }
        var appUserDTO = appUser.Adapt<AppUserDTO>();
        return new SuccessDataResult<AppUserDTO>(appUserDTO, "AppUser could found be successfully!");
    }

    public async Task<IResult> ResetPasswordAsync(AppUserResetPasswordDTO adminResetPasswordDTO)
    {
        var identityUser = await _userService.FindByEmailAsync(adminResetPasswordDTO.Email);
        if (identityUser == null)
        {
            return new ErrorResult("AppUser could not be founded!");
        }

        var result = await _userService.ResetPasswordAsync(identityUser, adminResetPasswordDTO.Code, adminResetPasswordDTO.Password);
        if (!result.Succeeded)
        {
            return new ErrorResult("Şifre sıfırlama başarısız!");
        }

        return new SuccessResult("Şifre başarıyla sıfırlandı!");
    }

    public async Task<IDataResult<AppUserForgotPasswordDTO>> ResetPasswordRequestAsync(string email)
    {
        var identityUser = await _userService.FindByEmailAsync(email);
        if (identityUser == null)
        {
            return new ErrorDataResult<AppUserForgotPasswordDTO>("AppUser could not be found!");
        }
        var code = await _userService.GeneratePasswordResetTokenAsync(identityUser);

        var appUserForgotPasswordDTO = new AppUserForgotPasswordDTO();
        appUserForgotPasswordDTO.Token = code;
        appUserForgotPasswordDTO.Result = IdentityResult.Success;
        appUserForgotPasswordDTO.Email = identityUser.Email;

        return new SuccessDataResult<AppUserForgotPasswordDTO>(appUserForgotPasswordDTO, "Şifre sıfırlama talebi başarıyla alındı!");
    }

    public async Task<IDataResult<AppUserDTO>> UpdateAsync(AppUserUpdateDTO appUserUpdateDTO)
    {
        var updatingAppUser = await _appUserRepository.GetByIdAsync(appUserUpdateDTO.Id);
        if (updatingAppUser == null)
        {
            return new ErrorDataResult<AppUserDTO>("AppUser could not be found!");
        }
        if (updatingAppUser.Email != appUserUpdateDTO.Email && await _userService.AnyAsync(x => x.Email == appUserUpdateDTO.Email))
        {
            return new ErrorDataResult<AppUserDTO>("Email is already taken!");
        }

        var identityUser = await _userService.FindByIdAsync(updatingAppUser.IdentityId);
        identityUser.Email = appUserUpdateDTO.Email;
        identityUser.NormalizedEmail = appUserUpdateDTO.Email.ToUpperInvariant();
        identityUser.UserName = appUserUpdateDTO.Email;
        identityUser.NormalizedUserName = appUserUpdateDTO.Email.ToUpperInvariant();

        DataResult<AppUserDTO> result = new ErrorDataResult<AppUserDTO>();

        var strategy = await _appUserRepository.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            var transectionScope = await _appUserRepository.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var identityResult = await _userService.UpdateUserAsync(identityUser);
                if (!identityResult.Succeeded)
                {
                    result = new ErrorDataResult<AppUserDTO>(identityResult.ToString());
                    transectionScope.Rollback();
                    return;
                }
                var updatedAppUser = appUserUpdateDTO.Adapt(updatingAppUser);

                await _appUserRepository.UpdateAsync(updatedAppUser);
                await _appUserRepository.SaveChangesAsync();
                result = new SuccessDataResult<AppUserDTO>("AppUser updated successfully!");
                transectionScope.Commit();
            }
            catch (Exception ex)
            {
                result = new ErrorDataResult<AppUserDTO>("AppUser could not be updated!" + ex.Message);
                transectionScope.Rollback();
            }
            finally
            {
                transectionScope.Dispose();
            }
        });
        return result;
    }

}
