using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Core.Common;
using SMS.Core.Enums;
using SMS.Core.Models.Account;
using SMS.Core.Models.Account.AddUser;
using SMS.Core.Services;
using System.Net;

namespace SMS.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : ApiBaseController
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }


        [HttpPost("add-student")]
        public async Task<IActionResult> AddStudent([FromBody] AddStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string>((int)HttpStatusCode.BadRequest, "Invalid request."));

            var result = await _adminService.AddStudentAsync(request);
            _logger.LogInformation("Admin added student with email: {Email} - Status: {Status}", request.Email, result.StatusCode);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add-teacher")]
        public async Task<IActionResult> AddTeacher([FromBody] AddTeacherRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string>((int)HttpStatusCode.BadRequest, "Invalid request."));

            var result = await _adminService.AddTeacherAsync(request);
            _logger.LogInformation("Admin added teacher with email: {Email} - Status: {Status}", request.Email, result.StatusCode);

            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("import-students")]
        public async Task<IActionResult> ImportStudentsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse<string>(400, "No file uploaded"));

            var result = await _adminService.ImportStudentsFromExcelAsync(file);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("import-teachers")]
        public async Task<IActionResult> ImportTeachersFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse<string>(400, "No file uploaded"));

            var result = await _adminService.ImportTeachersFromExcelAsync(file);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("export-teachers")]
        public async Task<IActionResult> ExportTeachers()
        {
            var result = await _adminService.ExportTeachersToExcelAsync();
            if (result.StatusCode != 200)
                return StatusCode(result.StatusCode, result);

            return File(result.Data,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Teachers.xlsx");
        }

        [HttpGet("export-students")]
        public async Task<IActionResult> ExportStudents()
        {
            var result = await _adminService.ExportStudentsToExcelAsync();
            if (result.StatusCode != 200)
                return StatusCode(result.StatusCode, result);

            return File(result.Data,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Students.xlsx");
        }

    }
}
