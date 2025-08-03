using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
                           , RoleManager<IdentityRole> roleManager
                           , IEmailService emailService
                           , IWebHostEnvironment env
                           , ILogger<AdminService> logger
                           , StoreContext storeContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _env = env;
            _logger = logger;
            _dbContext = storeContext;
        }

        private string GenerateSecurePassword(string role)
        {
            return role.ToLower() switch
            {
                "student" => "Student@123",
                "teacher" => "Teacher@123",
                _ => "Default@123"
            };
        }


        #region AddStudentAsync
        //public async Task<ApiResponse<string>> AddStudentAsync(AddStudentRequest request)
        //{
        //    try
        //    {
        //        if (request.Password != request.ConfirmPassword)
        //        {
        //            _logger.LogWarning("Password and ConfirmPassword do not match.");
        //            return new ApiResponse<string>(400, "Password and ConfirmPassword do not match.");
        //        }

        //        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        //        if (existingUser != null)
        //            return new ApiResponse<string>(400, "User already exists with the provided email.");

        //        var user = new AppUser
        //        {
        //            FirstName = request.FirstName,
        //            LastName = request.LastName,
        //            Email = request.Email,
        //            UserName = request.Email,
        //            EmailConfirmed = true,
        //            UserRole = UserRole.Student
        //        };

        //        var result = await _userManager.CreateAsync(user, request.Password);
        //        if (!result.Succeeded)
        //        {
        //            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        //            return new ApiResponse<string>(500, $"Failed to create user: {errors}");
        //        }

        //        var roleExists = await _roleManager.RoleExistsAsync(UserRole.Student.ToString());
        //        if (!roleExists)
        //            return new ApiResponse<string>(404, $"Role '{UserRole.Student}' does not exist.");

        //        var roleResult = await _userManager.AddToRoleAsync(user, UserRole.Student.ToString());
        //        if (!roleResult.Succeeded)
        //        {
        //            var errorMsg = string.Join(", ", roleResult.Errors.Select(e => e.Description));
        //            return new ApiResponse<string>(500, $"Failed to assign role: {errorMsg}");
        //        }

        //        var student = new Student
        //        {
        //            AppUserId = user.Id,
        //            DateOfBirth = request.DateOfBirth
        //        };

        //        await _dbContext.Students.AddAsync(student);
        //        await _dbContext.SaveChangesAsync();

        //        _logger.LogInformation($"Student added successfully: {user.Email}");

        //        return new ApiResponse<string>(200, "Student added successfully.", user.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while adding student.");
        //        return new ApiResponse<string>(500, "An error occurred while adding the student.");
        //    }
        //} 
        #endregion
        public async Task<ApiResponse<string>> AddStudentAsync(AddStudentRequest request)
        {
            _logger.LogInformation("Starting student creation...");

            if (!request.Email.Contains("@") || !request.Email.Contains("."))
                return new ApiResponse<string>(400, "Invalid email format.");

            if (request.Password != request.ConfirmPassword)
                return new ApiResponse<string>(400, "Password and confirmation password do not match.");

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return new ApiResponse<string>(400, "Email is already registered.");

            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = true,
                UserRole = UserRole.Student
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return new ApiResponse<string>(500, $"Failed to create student: {errors}");
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, UserRole.Student.ToString());
            if (!addToRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                return new ApiResponse<string>(500, $"Failed to assign role to student: {errors}");
            }

            var student = new Student
            {
                AppUserId = user.Id,
                DateOfBirth = request.DateOfBirth
            };

            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();

            try
            {
                var templatePath = Path.Combine(_env.WebRootPath, "HTML", "WelcomeEmail.html");
                var htmlBody = await File.ReadAllTextAsync(templatePath);

                htmlBody = htmlBody
                    .Replace("{{FullName}}", $"{user.FirstName} {user.LastName}")
                    .Replace("{{Email}}", user.Email)
                    .Replace("{{Password}}", request.Password)
                    .Replace("{{Role}}", "Student")
                    .Replace("{{DateOfBirth}}", request.DateOfBirth.ToString("yyyy-MM-dd"));

                await _emailService.SendAsync(new EmailMessage
                {
                    To = user.Email,
                    Subject = "Welcome to Student Management System",
                    Body = htmlBody,
                    IsHtml = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send welcome email");
            }

            return new ApiResponse<string>(200, "Student created successfully.");
        }



        public async Task<ApiResponse<string>> AddTeacherAsync(AddTeacherRequest request)
        {
            _logger.LogInformation("Starting teacher creation...");

            if (!request.Email.Contains("@") || !request.Email.Contains("."))
                return new ApiResponse<string>(400, "Invalid email format.");

            if (request.Password != request.ConfirmPassword)
                return new ApiResponse<string>(400, "Password and confirmation password do not match.");

            var department = await _dbContext.Departments.FindAsync(request.DepartmentId);
            if (department == null)
                return new ApiResponse<string>(404, "Department not found.");

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return new ApiResponse<string>(400, "Email is already registered.");

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
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return new ApiResponse<string>(500, $"Failed to create teacher: {errors}");
            }

            //await _userManager.AddToRoleAsync(user, UserRole.Teacher.ToString());
            var addToRoleResult = await _userManager.AddToRoleAsync(user, UserRole.Teacher.ToString());
            if (!addToRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                return new ApiResponse<string>(500, $"Failed to assign role to teacher: {errors}");
            }

            var teacher = new Teacher
            {
                AppUserId = user.Id,
                DepartmentId = request.DepartmentId
            };

            await _dbContext.Teachers.AddAsync(teacher);
            await _dbContext.SaveChangesAsync();

            try
            {
                var templatePath = Path.Combine(_env.WebRootPath, "HTML", "WelcomeEmail.html");
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
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send welcome email");
            }

            return new ApiResponse<string>(200, "Teacher created successfully.");
        }

        #region MyRegion
        //public async Task<ApiResponse<string>> ImportStudentsFromExcelAsync(IFormFile file)
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //            return new ApiResponse<string>(400, "No file uploaded");

        //        using var stream = new MemoryStream();
        //        await file.CopyToAsync(stream);
        //        stream.Position = 0;

        //        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //        using var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
        //        var result = reader.AsDataSet();
        //        var table = result.Tables[0];

        //        int addedCount = 0;

        //        for (int i = 1; i < table.Rows.Count; i++)
        //        {
        //            try
        //            {
        //                var firstName = table.Rows[i][0]?.ToString();
        //                var lastName = table.Rows[i][1]?.ToString();
        //                var email = table.Rows[i][2]?.ToString();
        //                var dob = DateOnly.Parse(table.Rows[i][3]?.ToString() ?? "");
        //                var password = table.Rows[i][4]?.ToString();

        //                var resultAdd = await AddStudentAsync(new AddStudentRequest
        //                {
        //                    FirstName = firstName!,
        //                    LastName = lastName!,
        //                    Email = email!,
        //                    Password = password!,
        //                    ConfirmPassword = password!,
        //                    DateOfBirth = dob
        //                });

        //                _logger.LogInformation(resultAdd.Message);
        //                addedCount++;
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogWarning(ex, "Failed to import student at row {Row}", i + 1);
        //            }
        //        }

        //        return new ApiResponse<string>(200, $"Students imported successfully. {addedCount} added.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while importing students from Excel");
        //        return new ApiResponse<string>(500, "An error occurred while importing students from Excel");
        //    }
        //}


        //public async Task<ApiResponse<string>> ImportTeachersFromExcelAsync(IFormFile file)
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //            return new ApiResponse<string>(400, "No file uploaded");

        //        using var stream = new MemoryStream();
        //        await file.CopyToAsync(stream);
        //        stream.Position = 0;

        //        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //        using var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
        //        var result = reader.AsDataSet();
        //        var table = result.Tables[0];

        //        int addedCount = 0;

        //        for (int i = 1; i < table.Rows.Count; i++)
        //        {
        //            try
        //            {
        //                var firstName = table.Rows[i][0]?.ToString();
        //                var lastName = table.Rows[i][1]?.ToString();
        //                var email = table.Rows[i][2]?.ToString();
        //                var departmentId = int.Parse(table.Rows[i][3]?.ToString() ?? "0");
        //                var password = table.Rows[i][4]?.ToString();

        //                var resultAdd = await AddTeacherAsync(new AddTeacherRequest
        //                {
        //                    FirstName = firstName!,
        //                    LastName = lastName!,
        //                    Email = email!,
        //                    Password = password!,
        //                    ConfirmPassword = password!,
        //                    DepartmentId = departmentId
        //                });

        //                _logger.LogInformation(resultAdd.Message);
        //                addedCount++;
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogWarning(ex, "Failed to import teacher at row {Row}", i + 1);
        //            }
        //        }

        //        return new ApiResponse<string>(200, $"Teachers imported successfully. {addedCount} added.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while importing teachers from Excel");
        //        return new ApiResponse<string>(500, "An error occurred while importing teachers from Excel");
        //    }
        //} 
        #endregion

        public async Task<ApiResponse<string>> ImportStudentsFromExcelAsync(IFormFile file)
        {
            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
                int addedCount = 0;

                foreach (var row in rows)
                {
                    var firstName = row.Cell(1).GetString().Trim();
                    var lastName = row.Cell(2).GetString().Trim();
                    var email = row.Cell(3).GetString().Trim();
                    var dobString = row.Cell(6).GetString().Trim();

                    if (!DateTime.TryParse(dobString, out var dob) || string.IsNullOrWhiteSpace(email))
                        continue;

                    var exists = await _dbContext.Users.AnyAsync(u => u.Email == email);
                    if (exists) continue;

                    var password = GenerateSecurePassword("Student"); 
                    var studentRequest = new AddStudentRequest
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        Password = password,
                        ConfirmPassword = password,
                        DateOfBirth = dob
                    };

                    var result = await AddStudentAsync(studentRequest);
                    if (result.StatusCode == 200) addedCount++;
                }

                return new ApiResponse<string>(200, $"Students imported successfully. {addedCount} added.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while importing students from Excel");
                return new ApiResponse<string>(500, "An error occurred while importing students");
            }
        }

        public async Task<ApiResponse<string>> ImportTeachersFromExcelAsync(IFormFile file)
        {
            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
                int addedCount = 0;

                foreach (var row in rows)
                {
                    var firstName = row.Cell(1).GetString().Trim();
                    var lastName = row.Cell(2).GetString().Trim();
                    var email = row.Cell(3).GetString().Trim();
                    var departmentName = row.Cell(4).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(email)) continue;

                    var exists = await _dbContext.Users.AnyAsync(u => u.Email == email);
                    if (exists) continue;

                    var department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Name == departmentName);
                    if (department == null) continue;

                    var password = GenerateSecurePassword("teacher");

                    var teacherRequest = new AddTeacherRequest
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        Password = password,
                        ConfirmPassword = password,
                        DepartmentId = department.Id
                    };

                    var result = await AddTeacherAsync(teacherRequest);
                    if (result.StatusCode == 200) addedCount++;
                }

                return new ApiResponse<string>(200, $"Teachers imported successfully. {addedCount} added.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while importing teachers from Excel");
                return new ApiResponse<string>(500, "An error occurred while importing teachers");
            }
        }



        public async Task<ApiResponse<byte[]>> ExportStudentsToExcelAsync()
        {
            try
            {
                var students = await _dbContext.Students.Include(s => s.AppUser).ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Students");

                worksheet.Cell(1, 1).Value = "FirstName";
                worksheet.Cell(1, 2).Value = "LastName";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "DateOfBirth";

                int row = 2;
                foreach (var student in students)
                {
                    worksheet.Cell(row, 1).Value = student.AppUser.FirstName;
                    worksheet.Cell(row, 2).Value = student.AppUser.LastName;
                    worksheet.Cell(row, 3).Value = student.AppUser.Email;
                    worksheet.Cell(row, 4).Value = student.DateOfBirth.ToShortDateString();
                    row++;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var data = stream.ToArray();

                return new ApiResponse<byte[]>(200, "Students exported successfully", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting students to Excel");
                return new ApiResponse<byte[]>(500, "An error occurred while exporting students to Excel");
            }
        }

        public async Task<ApiResponse<byte[]>> ExportTeachersToExcelAsync()
        {
            try
            {
                var teachers = await _dbContext.Teachers.Include(t => t.AppUser).Include(t => t.Department).ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Teachers");

                worksheet.Cell(1, 1).Value = "FirstName";
                worksheet.Cell(1, 2).Value = "LastName";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Department";

                int row = 2;
                foreach (var teacher in teachers)
                {
                    worksheet.Cell(row, 1).Value = teacher.AppUser.FirstName;
                    worksheet.Cell(row, 2).Value = teacher.AppUser.LastName;
                    worksheet.Cell(row, 3).Value = teacher.AppUser.Email;
                    worksheet.Cell(row, 4).Value = teacher.Department?.Name;
                    row++;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var data = stream.ToArray();

                return new ApiResponse<byte[]>(200, "Teachers exported successfully", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting teachers to Excel");
                return new ApiResponse<byte[]>(500, "An error occurred while exporting teachers to Excel");
            }
        }





    }

}
