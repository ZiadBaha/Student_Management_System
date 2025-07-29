using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Core.DTOs.Department;
using SMS.Repository.Services;

namespace SMS.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminDepartmentController : ApiBaseController
    {
        private readonly IDepartmentService _adminDepartmentService;

        public AdminDepartmentController(IDepartmentService adminDepartmentService)
        {
            _adminDepartmentService = adminDepartmentService;
        }

        [HttpPost("CreateDepartment")]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
        {
            var result = await _adminDepartmentService.CreateDepartmentAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("UpdateDepartment")]
        public async Task<IActionResult> UpdateDepartment([FromBody] UpdateDepartmentDto dto)
        {
            var result = await _adminDepartmentService.UpdateDepartmentAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("DeleteDepartment/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _adminDepartmentService.DeleteDepartmentAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetAllDepartments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var result = await _adminDepartmentService.GetAllDepartmentsAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetDepartmentById/{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var result = await _adminDepartmentService.GetDepartmentByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetDepartmentTeachers/{departmentId}")]
        public async Task<IActionResult> GetDepartmentTeachers(int departmentId)
        {
            var result = await _adminDepartmentService.GetDepartmentTeachersAsync(departmentId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
