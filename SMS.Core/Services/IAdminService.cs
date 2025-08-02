using Microsoft.AspNetCore.Http;
using SMS.Core.Common;
using SMS.Core.Enums;
using SMS.Core.Models.Account;
using SMS.Core.Models.Account.AddUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Services
{
    public interface IAdminService
    {
        Task<ApiResponse<string>> AddStudentAsync(AddStudentRequest request);
        Task<ApiResponse<string>> AddTeacherAsync(AddTeacherRequest request);
        Task<ApiResponse<string>> ImportTeachersFromExcelAsync(IFormFile file);
        Task<ApiResponse<string>> ImportStudentsFromExcelAsync(IFormFile file);
        Task<ApiResponse<byte[]>> ExportTeachersToExcelAsync();
        Task<ApiResponse<byte[]>> ExportStudentsToExcelAsync();
        //Task<ApiResponse<string>> AddUserAsync(RegisterRequest request, UserRole role);
    }
}
