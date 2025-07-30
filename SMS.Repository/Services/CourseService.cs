using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Core.Common;
using SMS.Core.DTOs.Course;
using SMS.Core.DTOs.Enrollment;
using SMS.Core.DTOs.Teacher;
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
    public class CourseService : ICourseService
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseService> _logger;

        public CourseService(StoreContext context, IMapper mapper, ILogger<CourseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CourseDto>>> GetAllCoursesAsync()
        {
            try
            {
                var courses = await _context.Courses
                    .Include(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                            .ThenInclude(s => s.AppUser)
                    .Include(c => c.CourseTeachers)
                        .ThenInclude(ct => ct.Teacher)
                            .ThenInclude(t => t.AppUser)
                    .ToListAsync();

                var dtoList = _mapper.Map<List<CourseDto>>(courses);
                return new ApiResponse<List<CourseDto>>(200, "Courses retrieved successfully", dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses");
                return new ApiResponse<List<CourseDto>>(500, "An error occurred while retrieving courses");
            }
        }

        public async Task<ApiResponse<CourseDto>> GetCourseByIdAsync(int id)
        {
            try
            {
                var course = await _context.Courses
                    .Include(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                            .ThenInclude(s => s.AppUser)
                    .Include(c => c.CourseTeachers)
                        .ThenInclude(ct => ct.Teacher)
                            .ThenInclude(t => t.AppUser)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (course == null)
                    return new ApiResponse<CourseDto>(404, "Course not found");

                var dto = _mapper.Map<CourseDto>(course);
                return new ApiResponse<CourseDto>(200, "Course retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course by ID");
                return new ApiResponse<CourseDto>(500, "An error occurred while retrieving the course");
            }
        }

        public async Task<ApiResponse<int>> CreateCourseAsync(CreateCourseDto dto)
        {
            try
            {
                var course = _mapper.Map<Course>(dto);
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                return new ApiResponse<int>(200, "Course created successfully", course.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return new ApiResponse<int>(500, "An error occurred while creating the course");
            }
        }

        public async Task<ApiResponse<bool>> UpdateCourseAsync(UpdateCourseDto dto)
        {
            try
            {
                var course = await _context.Courses.FindAsync(dto.Id);
                if (course == null)
                    return new ApiResponse<bool>(404, "Course not found");

                _mapper.Map(dto, course);
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>(200, "Course updated successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course");
                return new ApiResponse<bool>(500, "An error occurred while updating the course");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCourseAsync(int id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return new ApiResponse<bool>(404, "Course not found");

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>(200, "Course deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course");
                return new ApiResponse<bool>(500, "An error occurred while deleting the course");
            }
        }

        public async Task<ApiResponse<List<EnrolledStudentDto>>> GetEnrolledStudentsAsync(int courseId)
        {
            try
            {
                var course = await _context.Courses
                    .Include(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                            .ThenInclude(s => s.AppUser)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                    return new ApiResponse<List<EnrolledStudentDto>>(404, "Course not found");

                var enrolledStudents = course.Enrollments.Select(e => new EnrolledStudentDto
                {
                    StudentId = e.StudentId,
                    Grade = e.Grade,
                    FirstName = e.Student.AppUser.FirstName,
                    LastName = e.Student.AppUser.LastName
                }).ToList();

                return new ApiResponse<List<EnrolledStudentDto>>(200, "Enrolled students retrieved successfully", enrolledStudents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrolled students");
                return new ApiResponse<List<EnrolledStudentDto>>(500, "An error occurred while retrieving enrolled students");
            }
        }

        public async Task<ApiResponse<List<TeacherInfoDto>>> GetCourseTeachersAsync(int courseId)
        {
            try
            {
                var course = await _context.Courses
                    .Include(c => c.CourseTeachers)
                        .ThenInclude(ct => ct.Teacher)
                            .ThenInclude(t => t.AppUser)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                    return new ApiResponse<List<TeacherInfoDto>>(404, "Course not found");

                var teacherDtos = course.CourseTeachers.Select(ct => new TeacherInfoDto
                {
                    TeacherId = ct.Teacher.AppUser.Id,
                    FirstName = ct.Teacher.AppUser.FirstName,
                    LastName = ct.Teacher.AppUser.LastName,
                    Email = ct.Teacher.AppUser.Email,
                    DepartmentId = ct.Teacher.DepartmentId
                }).ToList();

                return new ApiResponse<List<TeacherInfoDto>>(200, "Course teachers retrieved successfully", teacherDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course teachers");
                return new ApiResponse<List<TeacherInfoDto>>(500, "An error occurred while retrieving course teachers");
            }
        }
    }
}
