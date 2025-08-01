using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Core.Common;
using SMS.Core.DTOs.Department;
using SMS.Core.Models.Entities;
using SMS.Core.Services;
using SMS.Repository.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Repository.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(StoreContext context, IMapper mapper, ILogger<DepartmentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<List<DepartmentDto>>> GetAllDepartmentsAsync()
        {
            try
            {
                var departments = await _context.Departments.ToListAsync();
                var dtoList = _mapper.Map<List<DepartmentDto>>(departments);
                return new ApiResponse<List<DepartmentDto>>(200, "Departments retrieved successfully", dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving departments");
                return new ApiResponse<List<DepartmentDto>>(500, "An error occurred while retrieving departments");
            }
        }

        public async Task<ApiResponse<DepartmentDto>> GetDepartmentByIdAsync(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                    return new ApiResponse<DepartmentDto>(404, "Department not found");

                var dto = _mapper.Map<DepartmentDto>(department);
                return new ApiResponse<DepartmentDto>(200, "Department retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving department by ID");
                return new ApiResponse<DepartmentDto>(500, "An error occurred while retrieving the department");
            }
        }

        public async Task<ApiResponse<int>> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            try
            {
                var department = _mapper.Map<Department>(dto);
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();

                return new ApiResponse<int>(200, "Department created successfully", department.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating department");
                return new ApiResponse<int>(500, "An error occurred while creating the department");
            }
        }

        public async Task<ApiResponse<bool>> UpdateDepartmentAsync(UpdateDepartmentDto dto)
        {
            try
            {
                var department = await _context.Departments.FindAsync(dto.Id);
                if (department == null)
                    return new ApiResponse<bool>(404, "Department not found");

                _mapper.Map(dto, department);
                _context.Departments.Update(department);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>(200, "Department updated successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating department");
                return new ApiResponse<bool>(500, "An error occurred while updating the department");
            }
        }

        public async Task<ApiResponse<bool>> DeleteDepartmentAsync(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                    return new ApiResponse<bool>(404, "Department not found");

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>(200, "Department deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting department");
                return new ApiResponse<bool>(500, "An error occurred while deleting the department");
            }
        }

        public async Task<ApiResponse<List<DepartmentTeacherDto>>> GetDepartmentTeachersAsync(int departmentId)
        {
            try
            {
                var department = await _context.Departments
                    .Include(d => d.Teachers)
                        .ThenInclude(t => t.AppUser)
                    .FirstOrDefaultAsync(d => d.Id == departmentId);

                if (department == null)
                    return new ApiResponse<List<DepartmentTeacherDto>>(404, "Department not found");

                var teacherDtos = department.Teachers.Select(t => new DepartmentTeacherDto
                {
                    Id = t.AppUser.Id,
                    FirstName = t.AppUser.FirstName,
                    LastName = t.AppUser.LastName,
                    Email = t.AppUser.Email
                }).ToList();

                return new ApiResponse<List<DepartmentTeacherDto>>(200, "Teachers retrieved successfully", teacherDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving department teachers");
                return new ApiResponse<List<DepartmentTeacherDto>>(500, "An error occurred while retrieving department teachers");
            }
        }


        public async Task<ApiResponse<byte[]>> ExportDepartmentsToExcelAsync()
        {
            try
            {
                var departments = await _context.Departments.ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Departments");

                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Name";

                for (int i = 0; i < departments.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = departments[i].Id;
                    worksheet.Cell(i + 2, 2).Value = departments[i].Name;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var excelData = stream.ToArray();

                return new ApiResponse<byte[]>(200, "Departments exported successfully", excelData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while exporting departments to Excel");
                return new ApiResponse<byte[]>(500, "An error occurred while exporting departments to Excel");
            }
        }

        public async Task<ApiResponse<string>> ImportDepartmentsFromExcelAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return new ApiResponse<string>(400, "No file uploaded");

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header
                foreach (var row in rows)
                {
                    var name = row.Cell(2).GetString();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        var department = new Department { Name = name };
                        _context.Departments.Add(department);
                    }
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Departments imported successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while importing departments from Excel");
                return new ApiResponse<string>(500, "An error occurred while importing departments from Excel");
            }
        }

    }
}
