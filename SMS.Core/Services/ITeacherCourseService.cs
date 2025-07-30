using SMS.Core.Common;
using SMS.Core.DTOs.TeacherCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Services
{
    public interface ITeacherCourseService
    {
        Task<ApiResponse<string>> AddCoursesToTeacherAsync(UpdateTeacherCoursesDto dto);
        Task<ApiResponse<string>> RemoveCoursesFromTeacherAsync(UpdateTeacherCoursesDto dto);
        Task<ApiResponse<GetTeacherCoursesDto>> GetTeacherCoursesAsync(string teacherId);
        Task<ApiResponse<string>> UpdateTeacherCoursesAsync(UpdateTeacherCoursesDto dto);
        Task<ApiResponse<List<GetTeacherCoursesDto>>> GetAllTeachersWithCoursesAsync();


    }
}
