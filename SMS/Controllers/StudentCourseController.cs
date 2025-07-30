using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Core.Common;
using SMS.Core.DTOs.StudentCourse;
using SMS.Core.Services;

namespace SMS.Controllers
{

    //[Authorize(Roles = "Admin")]
    public class StudentCourseController : ApiBaseController
    {
        private readonly IStudentCourseService _studentCourseService;
        private readonly ILogger<StudentCourseController> _logger;

        public StudentCourseController(IStudentCourseService studentCourseService, ILogger<StudentCourseController> logger)
        {
            _studentCourseService = studentCourseService;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse<string>>> AddCoursesToStudent([FromBody] AddCoursesToStudentDto dto)
        {
            var result = await _studentCourseService.AddCoursesToStudentAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("remove")]
        public async Task<ActionResult<ApiResponse<string>>> RemoveCoursesFromStudent([FromBody] RemoveCoursesFromStudentDto dto)
        {
            var result = await _studentCourseService.RemoveCoursesFromStudentAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<ApiResponse<GetStudentCoursesDto>>> GetStudentCourses(string studentId)
        {
            var result = await _studentCourseService.GetStudentCoursesAsync(studentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateStudentCourses([FromBody] UpdateStudentCoursesDto dto)
        {
            var result = await _studentCourseService.UpdateStudentCoursesAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
