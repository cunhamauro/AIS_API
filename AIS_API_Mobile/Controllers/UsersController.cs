using AIS_API_Mobile.Data.Entities;
using AIS_API_Mobile.HelperClasses;
using AIS_API_Mobile.Helpers;
using AIS_API_Mobile.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AIS_API_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public UsersController(IConfiguration configuration, IUserHelper userHelper, IImageHelper imageHelper, IMailHelper mailHelper)
        {
            _configuration = configuration;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            if (user != null)
            {
                return NotFound("There is already an user with this email!");
            }

            user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

            var registerResult = await _userHelper.AddUserAsync(user, model.Password);

            if (registerResult != IdentityResult.Success)
            {
                return BadRequest("Failed to register account!");
            }

            await _userHelper.AddUserToRoleAsync(user, "Client");

            string confirmationToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

            string emailBody = $"<h3>ACCOUNT CONFIRMATION TOKEN: </h3><p>{confirmationToken}</p>";

            Response mailResponse = await _mailHelper.SendEmailAsync(user.Email, "ACCOUNT CONFIRMATION", emailBody, null, null, null);

            if (!mailResponse.IsSuccess)
            {
                return BadRequest("Failed to mail account confirmation token!");
            }

            return Ok($"Account confirmation token sent to {user.Email}");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AccountConfirmation([FromBody] AccountConfirmationModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            var confirmationResult = await _userHelper.ConfirmEmailAsync(user, model.Token);

            if (confirmationResult != IdentityResult.Success)
            {
                return BadRequest("Account confirmation failed!");
            }

            return Ok("Account confirmation was successful!");

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            var loginResult = await _userHelper.ValidatePasswordAsync(user, model.Password);

            if (!loginResult.Succeeded)
            {
                return BadRequest("Login failed!");
            }

            var key = _configuration["JWT:Key"] ?? throw new ArgumentNullException("JWT:Key", "JWT:Key cannot be null.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new ObjectResult(new
            {
                AccessToken = jwt,
                TokenType = "bearer",
                UserId = user.Id,
                UserEmail = user.Email
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserInfo()
        {
            // Get user email from claims passed through bearer
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Get the user
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            return Ok(user);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserImage()
        {
            // Get user email from claims passed through bearer
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Get the user
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            var userImage = new { user.ImageUrl };

            return Ok(userImage);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadUserPhoto(IFormFile imageFile)
        {
            // Get user email from claims passed through bearer
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Get the user
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            string imagePath = user.ImageUrl;

            if (imageFile != null && imageFile.Length > 0)
            {
                imagePath = await _imageHelper.UploadImageAsync(imageFile, user.Id, "userimages");

                user.ImageUrl = imagePath;

                await _userHelper.UpdateUserAsync(user);

                return Ok("Image uploaded successfully!");
            }

            return BadRequest("Image failed to upload!");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateUser(UpdateUserModel model)
        {
            // Get user email from claims passed through bearer
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Get the user
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            if (model.FirstName != null)
            {
                user.FirstName = model.FirstName;
            }

            if (model.LastName != null)
            {
                user.LastName = model.LastName;
            }

            if (model.PhoneNumber != null)
            {
                user.PhoneNumber = model.PhoneNumber;
            }

            await _userHelper.UpdateUserAsync(user);

            return Ok("User updated successfully!");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordModel model)
        {
            // Get user email from claims passed through bearer
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Get the user
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            var validationResult = await _userHelper.ValidatePasswordAsync(user, model.CurrentPassword);

            if (!validationResult.Succeeded)
            {
                return BadRequest("The current password is incorrect!");
            }

            var updatePasswordResult = await _userHelper.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!updatePasswordResult.Succeeded)
            {
                return BadRequest("The password could not be updated!");
            }

            return Ok("Password updated successfully!");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            string passwordResetToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

            string emailBody = $"<h3>PASSWORD RESET TOKEN: </h3><p>{passwordResetToken}</p>";

            Response mailResponse = await _mailHelper.SendEmailAsync(user.Email, "PASSWORD RECOVERY", emailBody, null, null, null);

            if (!mailResponse.IsSuccess)
            {
                return BadRequest("Failed to mail password reset token!");
            }

            return Ok($"Password reset token sent to {user.Email}");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            var resetPasswordResult = await _userHelper.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!resetPasswordResult.Succeeded)
            {
                return BadRequest("The password could not be configured!");
            }

            return Ok("The password was successfully configured!");
        }
    }
}
