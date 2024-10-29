using AIS_API_Mobile.Data.Entities;
using AIS_API_Mobile.Data.Repositories;
using AIS_API_Mobile.Helpers;
using AIS_API_Mobile.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIS_API_Mobile.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAirportRepository _airportRepository;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IFlightRepository _flightRepository;

        public UserHelper(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IAirportRepository airportRepository, IAircraftRepository aircraftRepository, IFlightRepository flightRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _airportRepository = airportRepository;
            _aircraftRepository = aircraftRepository;
            _flightRepository = flightRepository;
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        /// <summary>
        /// Check if an User is registered in any of the Entities
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>User in Entities?</returns>
        //public async Task<bool> UserInEntities(User user)
        //{
        //    bool userInAircrafts = await _aircraftRepository.GetAll().Include(a => a.User).Where(a => a.User.Id == user.Id).AnyAsync();
        //    bool userInAirports = await _airportRepository.GetAll().Include(a => a.User).Where(a => a.User.Id == user.Id).AnyAsync();
        //    bool userInFlights = await _flightRepository.GetAll().Include(a => a.User).Where(a => a.User.Id == user.Id).AnyAsync();

        //    if (userInAircrafts || userInAirports || userInFlights)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Deletes the user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Task</returns>
        public async Task DeleteUserAsync(User user)
        {
            await _userManager.DeleteAsync(user);
        }

        /// <summary>
        /// Updates the user email & username & the normalized versions
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newEmail">New email</param>
        /// <returns>Task</returns>
        //public async Task UpdateUserEmailAndUsernameAsync(User user, string newEmail)
        //{
        //    var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        //    await _userManager.ChangeEmailAsync(user, newEmail, token);
        //    await _userManager.SetUserNameAsync(user, newEmail);
        //    await _userManager.UpdateNormalizedEmailAsync(user);
        //    await _userManager.UpdateNormalizedUserNameAsync(user);
        //}

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        //public async Task<SignInResult> LoginAsync(LoginModel model)
        //{
        //    return await _signInManager.PasswordSignInAsync(
        //        model.Email,
        //        model.Password,
        //        true,
        //        true);
        //}

        //public async Task LogoutAsync()
        //{
        //    await _signInManager.SignOutAsync();
        //}

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        //public async Task CheckRoleAsync(string roleName)
        //{
        //    var roleExists = await _roleManager.RoleExistsAsync(roleName);

        //    if (!roleExists)
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole
        //        {
        //            Name = roleName,
        //        });
        //    }
        //}

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task RemoveUserFromRoleAsync(User user, string roleName)
        {
            await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        //public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        //{
        //    return await _userManager.IsInRoleAsync(user, roleName);
        //}

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, true);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}
