using SMS.Core.Common;
using SMS.Core.DTOs.StudentCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Services
{
    public interface IStudentCourseService
    {
        Task<ApiResponse<string>> AddCoursesToStudentAsync(AddCoursesToStudentDto dto);
        Task<ApiResponse<string>> RemoveCoursesFromStudentAsync(RemoveCoursesFromStudentDto dto);
        Task<ApiResponse<GetStudentCoursesDto>> GetStudentCoursesAsync(string studentId);
        Task<ApiResponse<string>> UpdateStudentCoursesAsync(UpdateStudentCoursesDto dto);
    }
}
