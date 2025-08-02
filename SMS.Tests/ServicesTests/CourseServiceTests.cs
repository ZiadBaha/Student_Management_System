using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SMS.Core.DTOs.Course;
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
    public class CourseServiceTests
    {
        private readonly StoreContext _context;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CourseService>> _loggerMock;
        private readonly CourseService _service;

        public CourseServiceTests()
        {
            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new StoreContext(options);
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CourseService>>();
            _service = new CourseService(_context, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllCoursesAsync_ShouldReturnAllCourses()
        {
            _context.Courses.Add(new Course { Id = 1, Title = "Math", Description = "Basic Math" });
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<List<CourseDto>>(It.IsAny<List<Course>>()))
                .Returns(new List<CourseDto> { new CourseDto { Id = 1, Title = "Math" } });

            var result = await _service.GetAllCoursesAsync();

            Assert.Equal(200, result.StatusCode);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetCourseByIdAsync_ShouldReturnCourse_WhenExists()
        {
            _context.Courses.Add(new Course { Id = 2, Title = "Physics" });
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<CourseDto>(It.IsAny<Course>()))
                .Returns(new CourseDto { Id = 2, Title = "Physics" });

            var result = await _service.GetCourseByIdAsync(2);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Physics", result.Data.Title);
        }

        [Fact]
        public async Task GetCourseByIdAsync_ShouldReturnNotFound_WhenNotExists()
        {
            var result = await _service.GetCourseByIdAsync(999);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task CreateCourseAsync_ShouldAddCourse()
        {
            var dto = new CreateCourseDto { Title = "Biology", Description = "About life" };
            var course = new Course { Title = dto.Title, Description = dto.Description };

            _mapperMock.Setup(m => m.Map<Course>(dto)).Returns(course);

            var result = await _service.CreateCourseAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data > 0);
        }

        [Fact]
        public async Task UpdateCourseAsync_ShouldUpdate_WhenCourseExists()
        {
            var course = new Course { Id = 10, Title = "Chemistry" };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var dto = new UpdateCourseDto { Id = 10, Title = "Organic Chemistry" };

            _mapperMock.Setup(m => m.Map(dto, It.IsAny<Course>()))
                .Callback<UpdateCourseDto, Course>((src, dest) => dest.Title = src.Title);

            var result = await _service.UpdateCourseAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task UpdateCourseAsync_ShouldReturnNotFound_WhenCourseMissing()
        {
            var dto = new UpdateCourseDto { Id = 100, Title = "Does not exist" };
            var result = await _service.UpdateCourseAsync(dto);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task DeleteCourseAsync_ShouldDelete_WhenCourseExists()
        {
            var course = new Course { Id = 20, Title = "History" };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteCourseAsync(20);

            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteCourseAsync_ShouldReturnNotFound_WhenCourseMissing()
        {
            var result = await _service.DeleteCourseAsync(1234);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task GetEnrolledStudentsAsync_ShouldReturnList_WhenCourseExists()
        {
            var course = new Course
            {
                Id = 30,
                Title = "Geography",
                Enrollments = new List<Enrollment>
            {
                new Enrollment
                {
                    StudentId = "1",
                    Grade = 95,
                    Student = new Student
                    {
                        AppUser = new AppUser { FirstName = "Ziad", LastName = "bahaa" }
                    }
                }
            }
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = await _service.GetEnrolledStudentsAsync(30);

            Assert.Equal(200, result.StatusCode);
            Assert.Single(result.Data);
            Assert.Equal("Ziad", result.Data[0].FirstName);
        }

        [Fact]
        public async Task GetEnrolledStudentsAsync_ShouldReturnNotFound_WhenCourseMissing()
        {
            var result = await _service.GetEnrolledStudentsAsync(9999);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task GetCourseTeachersAsync_ShouldReturnList_WhenCourseExists()
        {
            var course = new Course
            {
                Id = 40,
                Title = "Programming",
                CourseTeachers = new List<CourseTeacher>
            {
                new CourseTeacher
                {
                    Teacher = new Teacher
                    {
                        DepartmentId = 1,
                        AppUser = new AppUser
                        {
                            Id = "t123",
                            FirstName = "ziad",
                            LastName = "bahaa",
                            Email = "ziad@gmail.com"
                        }
                    }
                }
            }
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = await _service.GetCourseTeachersAsync(40);

            Assert.Equal(200, result.StatusCode);
            Assert.Single(result.Data);
            Assert.Equal("Sara", result.Data[0].FirstName);
        }

        [Fact]
        public async Task GetCourseTeachersAsync_ShouldReturnNotFound_WhenCourseMissing()
        {
            var result = await _service.GetCourseTeachersAsync(8888);
            Assert.Equal(404, result.StatusCode);
        }
    }
}
