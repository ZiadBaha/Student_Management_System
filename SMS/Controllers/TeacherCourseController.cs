using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Core.Common;
using SMS.Core.DTOs.TeacherCourse;
using SMS.Core.Services;

namespace SMS.Controllers
{
    //[Authorize(Roles ="Admin")]
    public class TeacherCourseController : ApiBaseController
    {
        private readonly ITeacherCourseService _teacherCourseService;
        private readonly ILogger<TeacherCourseController> _logger;

        public TeacherCourseController(ITeacherCourseService teacherCourseService, ILogger<TeacherCourseController> logger)
        {
            _teacherCourseService = teacherCourseService;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse<string>>> AddCoursesToTeacher([FromBody] UpdateTeacherCoursesDto dto)
        {
            var result = await _teacherCourseService.AddCoursesToTeacherAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("remove")]
        public async Task<ActionResult<ApiResponse<string>>> RemoveCoursesFromTeacher([FromBody] UpdateTeacherCoursesDto dto)
        {
            var result = await _teacherCourseService.RemoveCoursesFromTeacherAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateTeacherCourses([FromBody] UpdateTeacherCoursesDto dto)
        {
            var result = await _teacherCourseService.UpdateTeacherCoursesAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{teacherId}")]
        public async Task<ActionResult<ApiResponse<GetTeacherCoursesDto>>> GetTeacherCourses(string teacherId)
        {
            var result = await _teacherCourseService.GetTeacherCoursesAsync(teacherId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("all-with-courses")]
        public async Task<ActionResult<ApiResponse<List<GetTeacherCoursesDto>>>> GetAllTeachersWithCourses()
        {
            var result = await _teacherCourseService.GetAllTeachersWithCoursesAsync();
            return StatusCode(result.StatusCode, result);
        }
    }
}
