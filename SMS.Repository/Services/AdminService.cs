using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Core.Common;
using SMS.Core.DTOs;
using SMS.Core.DTOs.Email;
using SMS.Core.Enums;
using SMS.Core.Models.Account.AddUser;
using SMS.Core.Models.Entities;
using SMS.Core.Models.Identity;
using SMS.Core.Services;
using SMS.Repository.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Repository.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminService> _logger;
        private readonly StoreContext _dbContext;


        public AdminService(UserManager<AppUser> userManager
                           ,RoleManager<IdentityRole> roleManager
                           ,IEmailService emailService
                           ,IWebHostEnvironment env
                           ,ILogger<AdminService> logger 
                           ,StoreContext storeContext )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _env = env;
            _logger = logger;
            _dbContext = storeContext;
        }

        public async Task<ApiResponse<string>> AddStudentAsync(AddStudentRequest request)
        {
            _logger.LogInformation("Starting student creation...");

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Email already in use: {Email}", request.Email);
                return new ApiResponse<string>(400, "Email is already in use.");
            }

            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = true,
                UserRole = UserRole.Student
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to create student user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return new ApiResponse<string>(500, "Failed to create student user.");
            }

            await _userManager.AddToRoleAsync(user, UserRole.Student.ToString());

            var student = new Student
            {
                AppUserId = user.Id,
                DateOfBirth = request.DateOfBirth.ToDateTime(TimeOnly.MinValue)
            };

            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();

            var templatePath = Path.Combine(_env.WebRootPath, "HTML", "WelcomeEmail.html");
            var htmlBody = await File.ReadAllTextAsync(templatePath);
            htmlBody = htmlBody
                .Replace("{{FullName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{Email}}", user.Email)
                .Replace("{{Password}}", request.Password)
                .Replace("{{Role}}", "Student");

            await _emailService.SendAsync(new EmailMessage
            {
                To = user.Email,
                Subject = "Welcome to Student Management System",
                Body = htmlBody,
                IsHtml = true
            });

            _logger.LogInformation("Student created successfully: {Email}", user.Email);

            return new ApiResponse<string>(200, "Student created successfully.");
        }

        public async Task<ApiResponse<string>> AddTeacherAsync(AddTeacherRequest request)
        {
            _logger.LogInformation("Starting teacher creation for: {Email}", request.Email);

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Email already taken: {Email}", request.Email);
                    return new ApiResponse<string>(400, "Email is already registered.");
                }

                var user = new AppUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserName = request.Email,
                    EmailConfirmed = true,
                    UserRole = UserRole.Teacher
                };

                var createResult = await _userManager.CreateAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create teacher user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    return new ApiResponse<string>(500, "Failed to create teacher user.");
                }

                await _userManager.AddToRoleAsync(user, UserRole.Teacher.ToString());

                var teacher = new Teacher
                {
                    AppUserId = user.Id,
                    DepartmentId = request.DepartmentId
                };

                await _dbContext.Teachers.AddAsync(teacher);
                await _dbContext.SaveChangesAsync();

                var templatePath = Path.Combine(_env.WebRootPath, "HtmlTemplates", "WelcomeEmail.html");
                var htmlBody = await File.ReadAllTextAsync(templatePath);

                htmlBody = htmlBody
                    .Replace("{{FullName}}", $"{user.FirstName} {user.LastName}")
                    .Replace("{{Email}}", user.Email)
                    .Replace("{{Password}}", request.Password)
                    .Replace("{{Role}}", "Teacher");

                await _emailService.SendAsync(new EmailMessage
                {
                    To = user.Email,
                    Subject = "Welcome to Student Management System",
                    Body = htmlBody,
                    IsHtml = true
                });

                _logger.LogInformation("Teacher created successfully: {Email}", user.Email);
                return new ApiResponse<string>(200, "Teacher created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating teacher.");
                return new ApiResponse<string>(500, "An error occurred while creating the teacher.");
            }
        }


        #region AddUser
        //public async Task<ApiResponse<string>> AddUserAsync(SMS.Core.Models.Account.RegisterRequest request, UserRole role)
        //{
        //    var existingUser = await _userManager.FindByEmailAsync(request.Email);
        //    if (existingUser != null)
        //        return new ApiResponse<string>(400, "البريد الإلكتروني مستخدم من قبل.");

        //    var user = new AppUser
        //    {
        //        FirstName = request.FirstName,
        //        LastName = request.LastName,
        //        Email = request.Email,
        //        UserName = request.Email,
        //        EmailConfirmed = true,
        //        UserRole = role
        //    };

        //    var result = await _userManager.CreateAsync(user, request.Password);
        //    if (!result.Succeeded)
        //        return new ApiResponse<string>(500, "فشل في إنشاء المستخدم.");

        //    await _userManager.AddToRoleAsync(user, role.ToString());

        //    // قراءة القالب
        //    var templatePath = Path.Combine(_env.WebRootPath, "HTML", "WelcomeEmail.html");
        //    var htmlBody = await File.ReadAllTextAsync(templatePath);
        //    htmlBody = htmlBody
        //        .Replace("{{FullName}}", $"{user.FirstName} {user.LastName}")
        //        .Replace("{{Email}}", user.Email)
        //        .Replace("{{Password}}", request.Password);

        //    // إرسال الإيميل
        //    await _emailService.SendAsync(new EmailMessage
        //    {
        //        To = user.Email,
        //        Subject = "Welcome to Student Management System",
        //        Body = htmlBody,
        //        IsHtml = true
        //    });

        //    return new ApiResponse<string>(200, "تم إنشاء المستخدم بنجاح.");
        //} 
        #endregion
    }

}
