using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SMS.Core.Models.Identity;
using SMS.Repository.Data.Context;
using SMS.Core.Models.Account;
using SMS.Core.Services;
using SMS.Core.Common;
using SMS.Core.Enums;
using Microsoft.AspNetCore.Hosting;


namespace SMS.Repository.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MailSettings _mailSettings;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AccountService> _logger;
        private readonly StoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public AccountService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<MailSettings> mailOptions,
            ITokenService tokenService,
            IOtpService otpService,
            IMemoryCache cache,
            ILogger<AccountService> logger , 
            StoreContext storeContext,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mailSettings = mailOptions.Value;
            _tokenService = tokenService;
            _otpService = otpService;
            _cache = cache;
            _logger = logger;
            _context = storeContext;
            _webHostEnvironment = webHostEnvironment;
        }



        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequest request)
        {
            AppUser? user;
            var input = request.Email.Trim();

            if (input.Contains("@"))
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Email == input);
            }
            else
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName == input);
            }

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return new ApiResponse<LoginResponseDto>(400, "بيانات الدخول غير صحيحة.");

            if (!user.EmailConfirmed)
                return new ApiResponse<LoginResponseDto>(403, "يرجى تأكيد البريد الإلكتروني أولاً.");

            var token = await _tokenService.CreateTokenAsync(user);

            var dto = new LoginResponseDto
            {
                Token = token,
                Role = user.UserRole.ToString(),
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty
            };

            return new ApiResponse<LoginResponseDto>(200, "تم تسجيل الدخول بنجاح.", dto);
        }



        public ApiResponse<string> VerifyOtp(VerifyOtpRequest request)
        {
            var result = _otpService.IsValidOtp(request.Email, request.Otp);
            return result ? new ApiResponse<string>(200, "OTP is valid.") : new ApiResponse<string>(400, "Invalid OTP.");
        }


        public async Task<ApiResponse<string>> SendEmailAsync(string to, string subject, string body, CancellationToken cancellation = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.DisplayedName, _mailSettings.Email));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.Port, SecureSocketOptions.StartTls, cancellation);
            await client.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password, cancellation);
            await client.SendAsync(message, cancellation);
            await client.DisconnectAsync(true, cancellation);

            return new ApiResponse<string>(200, "Email sent successfully.");
        }

        public async Task<ApiResponse<string>> ConfirmUserEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<string>(404, "User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded
                ? new ApiResponse<string>(200, "Email confirmed successfully.")
                : new ApiResponse<string>(400, "Invalid or expired token.");
        }

        public async Task<ApiResponse<string>> UpdateUserInfoAsync(string userId, UpdateUserInfoDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<string>(404, "User not found.");

            bool isChanged = false;
            var changedFields = new List<string>();

            if (user.FirstName != dto.FirstName)
            {
                changedFields.Add($"First Name: {user.FirstName} → {dto.FirstName}");
                user.FirstName = dto.FirstName;
                isChanged = true;
            }

            if (user.LastName != dto.LastName)
            {
                changedFields.Add($"Last Name: {user.LastName} → {dto.LastName}");
                user.LastName = dto.LastName;
                isChanged = true;
            }

            if (user.Email != dto.Email)
            {
                changedFields.Add($"Email: {user.Email} → {dto.Email}");
                user.Email = dto.Email;
                user.EmailConfirmed = false;
                isChanged = true;
            }

            if (!isChanged)
                return new ApiResponse<string>(200, "No changes detected.");

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new ApiResponse<string>(500, "Failed to update user.");

            var changeSummaryHtml = string.Join("<br/>", changedFields);


            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "HTML", "UserUpdatedEmail.html");
            var emailTemplate = await File.ReadAllTextAsync(templatePath);


            var emailBody = emailTemplate
                .Replace("{{FullName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{ChangeSummary}}", changeSummaryHtml);

            await SendEmailAsync(user.Email, "Your account information has been updated", emailBody);

            return new ApiResponse<string>(200, "User updated and notification sent.");
        }

        public async Task<ApiResponse<string>> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ApiResponse<string>(404, "User not found.");

            var otp = _otpService.GenerateOtp(email);

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "HTML", "ResetPasswordOtpTemplate.html");
            var emailTemplate = await File.ReadAllTextAsync(templatePath);


            var emailBody = emailTemplate
                .Replace("{{UserName}}", user.FirstName)
                .Replace("{{Otp}}", otp);

            await SendEmailAsync(email, "Reset Password OTP", emailBody);

            return new ApiResponse<string>(200, "OTP sent to email.");
        }


        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new ApiResponse<string>(404, "User not found.");

            if (!_cache.TryGetValue(request.Email, out bool validOtp) || !validOtp)
                return new ApiResponse<string>(403, "OTP not verified.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

            return result.Succeeded
                ? new ApiResponse<string>(200, "Password reset successful.")
                : new ApiResponse<string>(500, "Reset failed.");
        }

        
        public async Task<ApiResponse<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<string>(404, "User not found.");

            if (request.NewPassword != request.ConfirmNewPassword)
                return new ApiResponse<string>(400, "New password and confirmation do not match.");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return new ApiResponse<string>(400, "Current password is incorrect or the new password is weak.");

            if (string.IsNullOrWhiteSpace(user.Email))
                return new ApiResponse<string>(400, "Cannot send email notification. Email is not available.");

            // Read HTML template
            string templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "HTML", "ChangePasswordEmail.html");
            string emailBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders
            emailBody = emailBody.Replace("{{UserName}}", user.FirstName);

            await SendEmailAsync(user.Email, "Password Changed Successfully", emailBody);

            return new ApiResponse<string>(200, "Password changed successfully.");
        }

        public async Task<ApiResponse<UserInfoDto>> GetUserInfoAsync(string userId)
        {
            _logger.LogInformation("Retrieving user info for userId: {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found with userId: {UserId}", userId);
                return new ApiResponse<UserInfoDto>(404, "User not found.");
            }

            var dto = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = user.UserRole.ToString()
            };

            if (user.UserRole == UserRole.Student)
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.AppUserId == userId);
                if (student != null)
                {
                    dto.DateOfBirth = student.DateOfBirth;
                }
            }
            else if (user.UserRole == UserRole.Teacher)
            {
                var teacher = await _context.Teachers
                    .Include(t => t.Department)
                    .FirstOrDefaultAsync(t => t.AppUserId == userId);

                if (teacher != null)
                {
                    dto.DepartmentId = teacher.DepartmentId;
                    dto.DepartmentName = teacher.Department?.Name ?? string.Empty;
                }
            }

            _logger.LogInformation("User info successfully retrieved for userId: {UserId}", userId);
            return new ApiResponse<UserInfoDto>(200, "User info retrieved successfully.", dto);
        }


        #region Get User Info
        //public async Task<ApiResponse<UserInfoDto>> GetUserInfoAsync(string userId)
        //{
        //    var user = await _userManager.Users
        //        .FirstOrDefaultAsync(u => u.Id == userId);

        //    if (user == null)
        //        return new ApiResponse<UserInfoDto>(404, "User not found.");

        //    var dto = new UserInfoDto
        //    {
        //        FirstName = user.FirstName ?? string.Empty,
        //        LastName = user.LastName ?? string.Empty,
        //        Email = user.Email ?? string.Empty
        //    };

        //    return new ApiResponse<UserInfoDto>(200, "User data retrieved successfully", dto);
        //} 
        #endregion
    }
}
