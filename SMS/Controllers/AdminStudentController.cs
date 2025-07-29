using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Core.DTOs.Student;
using SMS.Core.Services;

namespace SMS.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminStudentController : ApiBaseController
    {
        private readonly IStudentService _studentService;

        public AdminStudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

      

        [HttpPut("UpdateStudent/{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] UpdateStudentDto dto)
        {
            var result = await _studentService.UpdateStudentAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents()
        {
            var result = await _studentService.GetAllStudentsAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetStudentById/{id}")]
        public async Task<IActionResult> GetStudentById(string id)
        {
            var result = await _studentService.GetStudentByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
