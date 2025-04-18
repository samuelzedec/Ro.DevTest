using Microsoft.AspNetCore.Identity;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Entities.Identity;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Infrastructure.Abstractions;

/// <summary>
/// This is a abstraction of the Identity library, creating methods that will interact with 
/// it to create and update users
/// </summary>
public class IdentityAbstractor(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<Role> roleManager)
    : IIdentityAbstractor
{
    
    #region SearchUsersMethods

    public async Task<User?> FindUserByEmailAsync(string email) 
        => await userManager.FindByEmailAsync(email);
    
    public async Task<User?> FindUserByNameAsync(string userName) 
        => await userManager.FindByNameAsync(userName);

    public async Task<User?> FindUserByIdAsync(string userId) 
        => await userManager.FindByIdAsync(userId);

    #endregion

    #region IdentityUserOperations

    public async Task<IdentityResult> CreateUserAsync(User partnerUser, string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException($"{nameof(password)} cannot be null or empty", nameof(password));
        

        if (string.IsNullOrEmpty(partnerUser.Email))
            throw new ArgumentException($"{nameof(User.Email)} cannot be null or empty", nameof(partnerUser));
        
        return await userManager.CreateAsync(partnerUser, password);
    }

    public async Task<IdentityResult> UpdateUserAsync(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        var existingUser = await userManager.FindByIdAsync(user.Id.ToString());
        if (existingUser == null)
            throw new KeyNotFoundException($"User with ID {user.Id} not found");
        
        existingUser.Name = user.Name;
        existingUser.UserName = user.UserName;
        existingUser.Email = user.Email;

        return await userManager.UpdateAsync(existingUser);
    }

    public async Task<IdentityResult> DeleteUser(User user) 
        => await userManager.DeleteAsync(user);
    
    public async Task<SignInResult> PasswordSignInAsync(User user, string password)
        => await signInManager.PasswordSignInAsync(user, password, false, false);

    #endregion

    #region IdentityRoleOperations

    public async Task<IList<string>> GetUserRolesAsync(User user) 
        => await userManager.GetRolesAsync(user);

    public async Task<IdentityResult> AddToRoleAsync(User user, UserRoles role)
    {
        if (await roleManager.RoleExistsAsync(role.ToString()) is false)
            await roleManager.CreateAsync(new Role { Name = role.ToString() });

        return await userManager.AddToRoleAsync(user, role.ToString());
    }
    
    #endregion
}