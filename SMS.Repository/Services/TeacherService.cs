using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Core.Common;
using SMS.Core.DTOs.Teacher;
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
    public class TeacherService : ITeacherService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<TeacherService> _logger;
        private readonly IMapper _mapper;
        private readonly StoreContext _context;

        public TeacherService(
            UserManager<AppUser> userManager,
            ILogger<TeacherService> logger,
            IMapper mapper,
            StoreContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ApiResponse<List<GetAllTeacherDto>>> GetAllTeachersAsync()
        {
            try
            {
                var teachers = await _context.Teachers
                    .Include(t => t.AppUser)
                    .ToListAsync();

                var dtoList = _mapper.Map<List<GetAllTeacherDto>>(teachers);
                return new ApiResponse<List<GetAllTeacherDto>>(200, "Teachers retrieved successfully", dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving teachers");
                return new ApiResponse<List<GetAllTeacherDto>>(500, "An error occurred while retrieving teachers");
            }
        }

        public async Task<ApiResponse<GetTeacherByIdDto>> GetTeacherByIdAsync(string id)
        {
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.AppUser)
                    .FirstOrDefaultAsync(t => t.AppUserId == id);

                if (teacher == null)
                    return new ApiResponse<GetTeacherByIdDto>(404, "Teacher not found");

                var dto = _mapper.Map<GetTeacherByIdDto>(teacher);
                return new ApiResponse<GetTeacherByIdDto>(200, "Teacher retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving teacher by ID");
                return new ApiResponse<GetTeacherByIdDto>(500, "An error occurred while retrieving the teacher");
            }
        }

        public async Task<ApiResponse<string>> UpdateTeacherAsync(string id, UpdateTeacherDto updateDto)
        {
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.AppUser)
                    .FirstOrDefaultAsync(t => t.AppUserId == id);

                if (teacher == null)
                    return new ApiResponse<string>(404, "Teacher not found");

                teacher.DepartmentId = updateDto.DepartmentId;
                teacher.AppUser.FirstName = updateDto.FirstName ?? teacher.AppUser.FirstName;
                teacher.AppUser.LastName = updateDto.LastName ?? teacher.AppUser.LastName;
                teacher.AppUser.Email = updateDto.Email ?? teacher.AppUser.Email;
                teacher.AppUser.UserName = updateDto.Email ?? teacher.AppUser.UserName;

                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Teacher updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating teacher");
                return new ApiResponse<string>(500, "An error occurred while updating the teacher");
            }
        }

        public async Task<ApiResponse<string>> DeleteTeacherAsync(string id)
        {
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.AppUser)
                    .FirstOrDefaultAsync(t => t.AppUserId == id);

                if (teacher == null)
                    return new ApiResponse<string>(404, "Teacher not found");

                _context.Teachers.Remove(teacher);
                await _userManager.DeleteAsync(teacher.AppUser);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>(200, "Teacher deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting teacher");
                return new ApiResponse<string>(500, "An error occurred while deleting the teacher");
            }
        }
    }
}
