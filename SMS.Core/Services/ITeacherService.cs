using SMS.Core.Common;
using SMS.Core.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Services
{
    public interface ITeacherService
    {
        Task<ApiResponse<List<GetAllTeacherDto>>> GetAllTeachersAsync();
        Task<ApiResponse<GetTeacherByIdDto>> GetTeacherByIdAsync(string id);
        Task<ApiResponse<string>> UpdateTeacherAsync(string id, UpdateTeacherDto updateDto);
        Task<ApiResponse<string>> DeleteTeacherAsync(string id);
    }
}
