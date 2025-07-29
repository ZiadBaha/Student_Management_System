using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Core.Common;
using SMS.Core.Models.Account;
using SMS.Core.Services;

namespace SMS.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        private string? GetUserId()
        {
            return User.FindFirst("_id")?.Value;
        }


       

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<string>>> Login([FromBody] LoginRequest request)
        {
            var result = await _accountService.LoginAsync(request);
            return StatusCode(result.StatusCode, result);
        }



        [HttpPost("forget-password")]
        public async Task<ActionResult<ApiResponse<string>>> ForgetPassword([FromQuery] string email)
        {
            var result = await _accountService.ForgetPasswordAsync(email);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("verify-otp")]
        public ActionResult<ApiResponse<string>> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = _accountService.VerifyOtp(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<string>>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _accountService.ResetPasswordAsync(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _accountService.ConfirmUserEmailAsync(userId, token);

            if (result.StatusCode == 200)
            {
                return Redirect("/HTML/email-confirmed-success.html");
            }

            return BadRequest("Invalid confirmation link.");
        }



        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse<string>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse<string>(401, "User not authenticated."));

            var result = await _accountService.ChangePasswordAsync(userId, request);
            return StatusCode(result.StatusCode, result);
        }


        [Authorize]
        [HttpPut("update-user-info")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateUserInfo([FromBody] UpdateUserInfoDto dto)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse<string>(401, "User not authenticated."));

            var result = await _accountService.UpdateUserInfoAsync(userId, dto);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-user-info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse<string>(401, "User not authenticated."));

            var result = await _accountService.GetUserInfoAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        


    


    }
}
