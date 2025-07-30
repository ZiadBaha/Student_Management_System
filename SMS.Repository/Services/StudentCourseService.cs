using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Core.Common;
using SMS.Core.DTOs.StudentCourse;
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
    public class StudentCourseService : IStudentCourseService
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentCourseService> _logger;

        public StudentCourseService(StoreContext context, IMapper mapper, ILogger<StudentCourseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> AddCoursesToStudentAsync(AddCoursesToStudentDto dto)
        {
            try
            {
                var studentExists = await _context.Students.AnyAsync(s => s.AppUserId == dto.StudentId);
                if (!studentExists)
                    return new ApiResponse<string>(404, "Student not found");

                foreach (var course in dto.Courses)
                {
                    bool alreadyEnrolled = await _context.Enrollments.AnyAsync(
                        e => e.Student.AppUserId == dto.StudentId && e.CourseId == course.CourseId
                    );

                    if (!alreadyEnrolled)
                    {
                        var enrollment = new Enrollment
                        {
                            StudentId = dto.StudentId,
                            CourseId = course.CourseId,
                            Grade = course.Grade ?? 0.0
                        };
                        _context.Enrollments.Add(enrollment);
                    }
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Courses added to student successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding courses to student");
                return new ApiResponse<string>(500, "An error occurred while adding courses to student");
            }
        }


        public async Task<ApiResponse<string>> RemoveCoursesFromStudentAsync(RemoveCoursesFromStudentDto dto)
        {
            try
            {
                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == dto.StudentId && dto.CourseIds.Contains(e.CourseId))
                    .ToListAsync();

                if (enrollments == null || enrollments.Count == 0)
                    return new ApiResponse<string>(404, "No matching enrollments found for this student");

                _context.Enrollments.RemoveRange(enrollments);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>(200, "Courses removed from student successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing courses from student");
                return new ApiResponse<string>(500, "An error occurred while removing courses from student");
            }
        }

        public async Task<ApiResponse<GetStudentCoursesDto>> GetStudentCoursesAsync(string studentId)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.Enrollments)
                        .ThenInclude(e => e.Course)
                    .FirstOrDefaultAsync(s => s.AppUserId == studentId);

                if (student == null)
                    return new ApiResponse<GetStudentCoursesDto>(404, "Student not found");

                var dto = _mapper.Map<GetStudentCoursesDto>(student);

                return new ApiResponse<GetStudentCoursesDto>(200, "Student courses retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while retrieving courses for student ID {studentId}");
                return new ApiResponse<GetStudentCoursesDto>(500, "An error occurred while retrieving student courses");
            }
        }

        public async Task<ApiResponse<string>> UpdateStudentCoursesAsync(UpdateStudentCoursesDto dto)
        {
            try
            {
                var student = await _context.Students
                   .Include(s => s.Enrollments)
                   .FirstOrDefaultAsync(s => s.AppUserId == dto.StudentId);


                if (student == null)
                    return new ApiResponse<string>(404, "Student not found");

                _context.Enrollments.RemoveRange(student.Enrollments);

                foreach (var course in dto.Courses)
                {
                    var enrollment = new Enrollment
                    {
                        StudentId = dto.StudentId,
                        CourseId = course.CourseId,
                        Grade = course.Grade
                    };
                    _context.Enrollments.Add(enrollment);
                }



                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Student courses updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating student courses");
                return new ApiResponse<string>(500, "An error occurred while updating student courses");
            }
        }
    }
}
