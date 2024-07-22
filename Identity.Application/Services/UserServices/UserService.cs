using Identity.Application.DTOs.AppUserDTOs;
using Identity.Application.Services.AppUserServices;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure.Repositories.AppUserRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Services.UserServices;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAppUserRepository _appUserRepository;


    public UserService(UserManager<IdentityUser> userManager, IAppUserRepository appUserRepository)
    {
        _userManager = userManager;
        _appUserRepository = appUserRepository;
    }
    public async Task<IdentityResult> CreateUserAsync(IdentityUser user, string password, Role role)
    {
        var result = await _userManager.CreateAsync(user, "Password.1");
        if (!result.Succeeded)
        {
            return result;
        }
        return await _userManager.AddToRoleAsync(user, role.ToString());
    }

    public async Task<bool> AnyAsync(Expression<Func<IdentityUser, bool>> expression)
    {
        return await _userManager.Users.AnyAsync(expression);
    }


    public async Task<IdentityResult> DeleteUserAsync(string identityId)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError()
            {
                Code = "Kullanıcı Bulunamadı",
                Description = "Kullanıcı Bulunamadı"
            });
        }
        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityUser?> FindByIdAsync(string identityId)
    {
        return await _userManager.FindByIdAsync(identityId);
    }

    public async Task<Guid> GetUserIdAsync(string identityId, string role)
    {
        AppUser? user = role switch
        {
            "Admin" => await _appUserRepository.GetByIdentityId(identityId),
            _ => null,
        };
        return user is null ? Guid.NewGuid() : user.Id;
    }

    public async Task<IdentityResult> UpdateUserAsync(IdentityUser user)
    {
        var updatingUser = await _userManager.FindByIdAsync(user.Id);
        if (updatingUser == null)
        {
            return IdentityResult.Failed(new IdentityError()
            {
                Code = "Güncellenecek User Bulunamadı",
                Description = "Güncellenecek User Bulunamadı"
            });
        }

        return await _userManager.UpdateAsync(user);
    }


    public async Task<IdentityResult> ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<IdentityUser?> FindByEmailAsync(string email)  //Email karşılık bir kullanıcı durumu sorgulama
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(IdentityUser user)  //usera özel reset token üretme 
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string newPassword)   //token şifreleme
    {
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

}
