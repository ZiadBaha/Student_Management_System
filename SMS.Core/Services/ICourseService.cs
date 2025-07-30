using SMS.Core.Common;
using SMS.Core.DTOs.Course;
using SMS.Core.DTOs.Enrollment;
using SMS.Core.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Services
{
    public interface ICourseService
    {
        Task<ApiResponse<List<CourseDto>>> GetAllCoursesAsync();
        Task<ApiResponse<CourseDto>> GetCourseByIdAsync(int id);
        Task<ApiResponse<int>> CreateCourseAsync(CreateCourseDto dto);
        Task<ApiResponse<bool>> UpdateCourseAsync(UpdateCourseDto dto);
        Task<ApiResponse<bool>> DeleteCourseAsync(int id);
        Task<ApiResponse<List<EnrolledStudentDto>>> GetEnrolledStudentsAsync(int courseId);
        Task<ApiResponse<List<TeacherInfoDto>>> GetCourseTeachersAsync(int courseId);
    }
}
