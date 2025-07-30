using SMS.Core.Common;
using SMS.Core.DTOs.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Services
{
    public interface IStudentService
    {
        Task<ApiResponse<List<GetAllStudentDto>>> GetAllStudentsAsync();
        Task<ApiResponse<GetStudentByIdDto>> GetStudentByIdAsync(string id);
        Task<ApiResponse<string>> UpdateStudentAsync(string id, UpdateStudentDto dto);
        Task<ApiResponse<string>> DeleteStudentAsync(string id);
    }
}
