using Microsoft.Win32;
using SMS.Core.Common;
using SMS.Core.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Services
{
    public interface IAccountService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequest request);
        Task<ApiResponse<string>> ForgetPasswordAsync(string email);
        ApiResponse<string> VerifyOtp(VerifyOtpRequest request);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ApiResponse<string>> SendEmailAsync(string to, string subject, string body, CancellationToken cancellation = default);
        Task<ApiResponse<string>> ConfirmUserEmailAsync(string userId, string token);
        Task<ApiResponse<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<ApiResponse<string>> UpdateUserInfoAsync(string userId, UpdateUserInfoDto dto);
        Task<ApiResponse<UserInfoDto>> GetUserInfoAsync(string userId);
        //Task<ApiResponse<string>> DeleteAccountAsync(string userId);
    }
}
