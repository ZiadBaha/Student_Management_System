using SMS.Core.Common;
using SMS.Core.DTOs.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Repository.Services
{
    public interface IDepartmentService
    {
        Task<ApiResponse<List<DepartmentDto>>> GetAllDepartmentsAsync();
        Task<ApiResponse<DepartmentDto>> GetDepartmentByIdAsync(int id);
        Task<ApiResponse<int>> CreateDepartmentAsync(CreateDepartmentDto dto);
        Task<ApiResponse<bool>> UpdateDepartmentAsync(UpdateDepartmentDto dto);
        Task<ApiResponse<bool>> DeleteDepartmentAsync(int id);
        Task<ApiResponse<List<DepartmentTeacherDto>>> GetDepartmentTeachersAsync(int departmentId);
    }
}
