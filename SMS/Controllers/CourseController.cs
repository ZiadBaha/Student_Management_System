using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Core.DTOs.Course;
using SMS.Core.Services;

namespace SMS.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CourseController : ApiBaseController
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(ICourseService courseService, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpPost("CreateCourse")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            var result = await _courseService.CreateCourseAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("UpdateCourse")]
        public async Task<IActionResult> UpdateCourse([FromBody] UpdateCourseDto dto)
        {
            var result = await _courseService.UpdateCourseAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("DeleteCourse/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await _courseService.GetAllCoursesAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetCourseById/{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var result = await _courseService.GetCourseByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetEnrolledStudents/{courseId}")]
        public async Task<IActionResult> GetEnrolledStudents(int courseId)
        {
            var result = await _courseService.GetEnrolledStudentsAsync(courseId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetCourseTeachers/{courseId}")]
        public async Task<IActionResult> GetCourseTeachers(int courseId)
        {
            var result = await _courseService.GetCourseTeachersAsync(courseId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
