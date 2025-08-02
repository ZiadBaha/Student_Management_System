using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SMS.Core.DTOs.Course;
using SMS.Core.DTOs.StudentCourse;
using SMS.Core.Models.Entities;
using SMS.Repository.Data.Context;
using SMS.Repository.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SMS.Tests.ServicesTests
{
    public class StudentCourseServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<StudentCourseService>> _loggerMock;
        private readonly StoreContext _context;
        private readonly StudentCourseService _service;

        public StudentCourseServiceTests()
        {
            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: "StudentCourseServiceTestDb")
                .Options;

            _context = new StoreContext(options);
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<StudentCourseService>>();
            _service = new StudentCourseService(_context, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddCoursesToStudentAsync_ShouldAddCourses()
        {
            var student = new Student { AppUserId = "student1" };
            var course = new Course { Id = 1, Title = "Math" };

            _context.Students.Add(student);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var dto = new AddCoursesToStudentDto
            {
                StudentId = "student1",
                Courses = new List<CourseGradeDto> { new CourseGradeDto { CourseId = 1, Grade = 95 } }
            };

            var result = await _service.AddCoursesToStudentAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.Single(_context.Enrollments);
        }

        [Fact]
        public async Task RemoveCoursesFromStudentAsync_ShouldRemoveCourses()
        {
            var student = new Student { AppUserId = "student2" };
            var enrollment = new Enrollment { CourseId = 2, StudentId = "student2" };

            _context.Students.Add(student);
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var dto = new RemoveCoursesFromStudentDto
            {
                StudentId = "student2",
                CourseIds = new List<int> { 2 }
            };

            var result = await _service.RemoveCoursesFromStudentAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.Empty(_context.Enrollments);
        }

        [Fact]
        public async Task GetStudentCoursesAsync_ShouldReturnCourses()
        {
            var student = new Student
            {
                AppUserId = "student3",
                Enrollments = new List<Enrollment>
            {
                new Enrollment { Course = new Course { Id = 3, Title = "Science" }, Grade = 88 }
            }
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var expectedDto = new GetStudentCoursesDto { StudentId = "student3" };
            _mapperMock.Setup(m => m.Map<GetStudentCoursesDto>(It.IsAny<Student>())).Returns(expectedDto);

            var result = await _service.GetStudentCoursesAsync("student3");

            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateStudentCoursesAsync_ShouldReplaceCourses()
        {
            var student = new Student
            {
                AppUserId = "student4",
                Enrollments = new List<Enrollment> { new Enrollment { CourseId = 4, Grade = 70 } }
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var dto = new UpdateStudentCoursesDto
            {
                StudentId = "student4",
                Courses = new List<UpdateCourseGradeDto> { new UpdateCourseGradeDto { CourseId = 5, Grade = 90 } }
            };

            var result = await _service.UpdateStudentCoursesAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.Single(_context.Enrollments);
            Assert.Equal(5, _context.Enrollments.First().CourseId);
        }
    }
}
