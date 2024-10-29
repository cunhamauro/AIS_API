using AIS_API_Mobile.Data.Entities;
using AIS_API_Mobile.Models;
using Microsoft.AspNetCore.Identity;

namespace AIS_API_Mobile.Helpers
{
    public interface IUserHelper
    {
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        //Task<bool> UserInEntities(User user);

        Task DeleteUserAsync(User user);

        //Task UpdateUserEmailAndUsernameAsync(User user, string newEmail);

        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        //Task<SignInResult> LoginAsync(LoginModel model);

        //Task LogoutAsync();

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

        //Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task RemoveUserFromRoleAsync(User user, string roleName);

        //Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<User> GetUserByIdAsync(string userId);
    }
}
