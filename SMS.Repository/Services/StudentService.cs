using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Core.Common;
using SMS.Core.DTOs.Student;
using SMS.Core.Models.Identity;
using SMS.Core.Services;
using SMS.Repository.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Repository.Services
{
    public class StudentService : IStudentService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<StudentService> _logger;
        private readonly IMapper _mapper;
        private readonly StoreContext _context;

        public StudentService(
            UserManager<AppUser> userManager,
            ILogger<StudentService> logger,
            IMapper mapper,
            StoreContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ApiResponse<List<GetAllStudentDto>>> GetAllStudentsAsync()
        {
            try
            {
                var students = await _context.Students
                    .Include(s => s.AppUser)
                    .ToListAsync();

                var dtoList = _mapper.Map<List<GetAllStudentDto>>(students);
                return new ApiResponse<List<GetAllStudentDto>>(200, "Students retrieved successfully", dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving students");
                return new ApiResponse<List<GetAllStudentDto>>(500, "An error occurred while retrieving students");
            }
        }

        public async Task<ApiResponse<GetStudentByIdDto>> GetStudentByIdAsync(string id)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.AppUser)
                    .FirstOrDefaultAsync(s => s.AppUserId == id);

                if (student == null)
                    return new ApiResponse<GetStudentByIdDto>(404, "Student not found");

                var dto = _mapper.Map<GetStudentByIdDto>(student);
                return new ApiResponse<GetStudentByIdDto>(200, "Student retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving student by ID");
                return new ApiResponse<GetStudentByIdDto>(500, "An error occurred while retrieving the student");
            }
        }

        public async Task<ApiResponse<string>> UpdateStudentAsync(string id, UpdateStudentDto dto)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.AppUser)
                    .FirstOrDefaultAsync(s => s.AppUserId == id);

                if (student == null)
                    return new ApiResponse<string>(404, "Student not found");

                student.DateOfBirth = dto.DateOfBirth;
                student.AppUser.FirstName = dto.FirstName ?? student.AppUser.FirstName;
                student.AppUser.LastName = dto.LastName ?? student.AppUser.LastName;
                student.AppUser.Email = dto.Email ?? student.AppUser.Email;
                student.AppUser.UserName = dto.Email ?? student.AppUser.UserName;

                await _context.SaveChangesAsync();

                return new ApiResponse<string>(200, "Student updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating student");
                return new ApiResponse<string>(500, "An error occurred while updating the student");
            }
        }

        public async Task<ApiResponse<bool>> DeleteStudentAsync(string id)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.AppUser)
                    .FirstOrDefaultAsync(s => s.AppUserId == id);

                if (student == null)
                    return new ApiResponse<bool>(404, "Student not found");

                _context.Students.Remove(student);
                await _userManager.DeleteAsync(student.AppUser);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>(200, "Student deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting student");
                return new ApiResponse<bool>(500, "An error occurred while deleting the student");
            }
        }
    }
}
