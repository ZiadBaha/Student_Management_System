using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SMS.Core.DTOs.TeacherCourse;
using SMS.Core.Models.Entities;
using SMS.Core.Models.Identity;
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
    public class TeacherCourseServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TeacherCourseService>> _loggerMock;
        private readonly StoreContext _context;
        private readonly TeacherCourseService _service;

        public TeacherCourseServiceTests()
        {
            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: "TeacherCourseServiceDb")
                .Options;

            _context = new StoreContext(options);
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TeacherCourseService>>();
            _service = new TeacherCourseService(_context, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddCoursesToTeacherAsync_ShouldAddCourses()
        {
            var teacher = new Teacher { Id = 1, AppUserId = "t1" };
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            var dto = new UpdateTeacherCoursesDto { TeacherId = "t1", CourseIds = new List<int> { 101, 102 } };

            var result = await _service.AddCoursesToTeacherAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, _context.CourseTeachers.Count());
        }

        [Fact]
        public async Task RemoveCoursesFromTeacherAsync_ShouldRemoveCourses()
        {
            var teacher = new Teacher { Id = 2, AppUserId = "t2" };
            _context.Teachers.Add(teacher);
            _context.CourseTeachers.AddRange(
                new CourseTeacher { TeacherId = 2, CourseId = 201 },
                new CourseTeacher { TeacherId = 2, CourseId = 202 }
            );
            await _context.SaveChangesAsync();

            var dto = new UpdateTeacherCoursesDto { TeacherId = "t2", CourseIds = new List<int> { 201 } };

            var result = await _service.RemoveCoursesFromTeacherAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.Single(_context.CourseTeachers);
        }

        [Fact]
        public async Task UpdateTeacherCoursesAsync_ShouldUpdateCourses()
        {
            var teacher = new Teacher { Id = 3, AppUserId = "t3" };
            _context.Teachers.Add(teacher);
            _context.CourseTeachers.Add(new CourseTeacher { TeacherId = 3, CourseId = 301 });
            await _context.SaveChangesAsync();

            var dto = new UpdateTeacherCoursesDto { TeacherId = "t3", CourseIds = new List<int> { 302, 303 } };

            var result = await _service.UpdateTeacherCoursesAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, _context.CourseTeachers.Count(ct => ct.TeacherId == 3));
        }

        [Fact]
        public async Task GetTeacherCoursesAsync_ShouldReturnCourses()
        {
            var teacher = new Teacher { Id = 4, AppUserId = "t4" };
            _context.Teachers.Add(teacher);
            _context.CourseTeachers.Add(new CourseTeacher
            {
                TeacherId = 4,
                CourseId = 401,
                Course = new Course { Id = 401, Title = "Math", Description = "Math Course" }
            });
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<GetTeacherCoursesDto>(It.IsAny<Teacher>()))
                .Returns(new GetTeacherCoursesDto { TeacherId = "t4", Courses = new List<TeacherCourseDto> { new TeacherCourseDto { CourseId = 401, Title = "Math", Description = "Math Course" } } });

            var result = await _service.GetTeacherCoursesAsync("t4");

            Assert.Equal(200, result.StatusCode);
            Assert.Single(result.Data.Courses);
        }

        [Fact]
        public async Task GetAllTeachersWithCoursesAsync_ShouldReturnAllTeachers()
        {
            _context.Teachers.Add(new Teacher
            {
                Id = 5,
                AppUserId = "t5",
                AppUser = new AppUser { Id = "t5", FirstName = "John", LastName = "Doe" },
                CourseTeachers = new List<CourseTeacher>
            {
                new CourseTeacher { Course = new Course { Id = 501, Title = "Physics", Description = "Physics Course" } }
            }
            });
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<List<GetTeacherCoursesDto>>(It.IsAny<List<Teacher>>()))
                .Returns(new List<GetTeacherCoursesDto>
                {
                new GetTeacherCoursesDto { TeacherId = "t5", Courses = new List<TeacherCourseDto> { new TeacherCourseDto { CourseId = 501, Title = "Physics" } } }
                });

            var result = await _service.GetAllTeachersWithCoursesAsync();

            Assert.Equal(200, result.StatusCode);
            Assert.Single(result.Data);
        }
    }

}
