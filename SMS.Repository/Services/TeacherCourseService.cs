using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Core.Common;
using SMS.Core.DTOs.TeacherCourse;
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
    public class TeacherCourseService : ITeacherCourseService
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TeacherCourseService> _logger;

        public TeacherCourseService(StoreContext context, IMapper mapper, ILogger<TeacherCourseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> AddCoursesToTeacherAsync(UpdateTeacherCoursesDto dto)
        {
            try
            {
                var teacherExists = await _context.Teachers.AnyAsync(t => t.AppUserId == dto.TeacherId);
                if (!teacherExists)
                    return new ApiResponse<string>(404, "Teacher not found");

                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.AppUserId == dto.TeacherId);

                foreach (var courseId in dto.CourseIds)
                {
                    bool alreadyAssigned = await _context.CourseTeachers.AnyAsync(
                        ct => ct.TeacherId == teacher.Id && ct.CourseId == courseId
                    );

                    if (!alreadyAssigned)
                    {
                        var courseTeacher = new CourseTeacher
                        {
                            TeacherId = teacher.Id,
                            CourseId = courseId
                        };
                        _context.CourseTeachers.Add(courseTeacher);
                    }
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Courses assigned to teacher successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while assigning courses to teacher");
                return new ApiResponse<string>(500, "An error occurred while assigning courses to teacher");
            }
        }


        public async Task<ApiResponse<string>> RemoveCoursesFromTeacherAsync(UpdateTeacherCoursesDto dto)
        {
            try
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.AppUserId == dto.TeacherId);
                if (teacher == null)
                    return new ApiResponse<string>(404, "Teacher not found");

                var courseTeachers = await _context.CourseTeachers
                    .Where(ct => ct.TeacherId == teacher.Id && dto.CourseIds.Contains(ct.CourseId))
                    .ToListAsync();

                if (!courseTeachers.Any())
                    return new ApiResponse<string>(404, "No matching course assignments found for this teacher");

                _context.CourseTeachers.RemoveRange(courseTeachers);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>(200, "Courses removed from teacher successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing courses from teacher");
                return new ApiResponse<string>(500, "An error occurred while removing courses from teacher");
            }
        }


        public async Task<ApiResponse<GetTeacherCoursesDto>> GetTeacherCoursesAsync(string teacherId)
        {
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.CourseTeachers)
                        .ThenInclude(ct => ct.Course)
                    .FirstOrDefaultAsync(t => t.AppUserId == teacherId);

                if (teacher == null)
                    return new ApiResponse<GetTeacherCoursesDto>(404, "Teacher not found");

                var dto = _mapper.Map<GetTeacherCoursesDto>(teacher);

                return new ApiResponse<GetTeacherCoursesDto>(200, "Teacher courses retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while retrieving courses for teacher ID {teacherId}");
                return new ApiResponse<GetTeacherCoursesDto>(500, "An error occurred while retrieving teacher courses");
            }
        }

        public async Task<ApiResponse<string>> UpdateTeacherCoursesAsync(UpdateTeacherCoursesDto dto)
        {
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.CourseTeachers)
                    .FirstOrDefaultAsync(t => t.AppUserId == dto.TeacherId);

                if (teacher == null)
                    return new ApiResponse<string>(404, "Teacher not found");

                _context.CourseTeachers.RemoveRange(teacher.CourseTeachers);

                foreach (var courseId in dto.CourseIds)
                {
                    var courseTeacher = new CourseTeacher
                    {
                        TeacherId = teacher.Id, 
                        CourseId = courseId
                    };
                    _context.CourseTeachers.Add(courseTeacher);
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Teacher courses updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating teacher courses");
                return new ApiResponse<string>(500, "An error occurred while updating teacher courses");
            }
        }


        public async Task<ApiResponse<List<GetTeacherCoursesDto>>> GetAllTeachersWithCoursesAsync()
        {
            try
            {
                var teachers = await _context.Teachers
                    .Include(t => t.AppUser)                
                    .Include(t => t.CourseTeachers)         
                        .ThenInclude(ct => ct.Course)       
                    .ToListAsync();

                var result = _mapper.Map<List<GetTeacherCoursesDto>>(teachers);

                return new ApiResponse<List<GetTeacherCoursesDto>>(200, "All teachers with their courses retrieved successfully", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting all teachers with courses");
                return new ApiResponse<List<GetTeacherCoursesDto>>(500, "An error occurred while retrieving teachers");
            }
        }
    }
}
