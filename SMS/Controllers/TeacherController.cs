using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Core.DTOs.Teacher;
using SMS.Core.Services;

namespace SMS.Controllers
{
    //[Authorize (Roles = "Admin")]
    public class TeacherController : ApiBaseController
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpPut("UpdateTeacher/{id}")]
        public async Task<IActionResult> UpdateTeacher(string id, [FromBody] UpdateTeacherDto dto)
        {
            var result = await _teacherService.UpdateTeacherAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("DeleteTeacher/{id}")]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            var result = await _teacherService.DeleteTeacherAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetAllTeachers")]
        public async Task<IActionResult> GetAllTeachers()
        {
            var result = await _teacherService.GetAllTeachersAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetTeacherById/{id}")]
        public async Task<IActionResult> GetTeacherById(string id)
        {
            var result = await _teacherService.GetTeacherByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}

