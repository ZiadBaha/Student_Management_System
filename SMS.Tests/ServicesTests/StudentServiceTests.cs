using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SMS.Core.DTOs.Student;
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
    public class StudentServiceTests
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<ILogger<StudentService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly StoreContext _context;
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new StoreContext(options);
            _userManagerMock = new Mock<UserManager<AppUser>>(
                Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _loggerMock = new Mock<ILogger<StudentService>>();
            _mapperMock = new Mock<IMapper>();

            _service = new StudentService(_userManagerMock.Object, _loggerMock.Object, _mapperMock.Object, _context);
        }

        [Fact]
        public async Task GetAllStudentsAsync_ShouldReturnStudents()
        {
            _context.Students.Add(new SMS.Core.Models.Entities.Student { Id = 1, AppUser = new AppUser { FirstName = "John" } });
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<List<GetAllStudentDto>>(It.IsAny<List<SMS.Core.Models.Entities.Student>>()))
                .Returns(new List<GetAllStudentDto> { new GetAllStudentDto { FirstName = "John" } });

            var result = await _service.GetAllStudentsAsync();

            Assert.Equal(200, result.StatusCode);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnStudent()
        {
            _context.Students.Add(new SMS.Core.Models.Entities.Student { Id = 2, AppUserId = "s1", AppUser = new AppUser { Id = "s1", FirstName = "Jane" } });
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<GetStudentByIdDto>(It.IsAny<SMS.Core.Models.Entities.Student>()))
                .Returns(new GetStudentByIdDto { FirstName = "Jane" });

            var result = await _service.GetStudentByIdAsync("s1");

            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Jane", result.Data.FirstName);
        }

        [Fact]
        public async Task UpdateStudentAsync_ShouldUpdateStudent()
        {
            var student = new SMS.Core.Models.Entities.Student { Id = 3, AppUserId = "s2", AppUser = new AppUser { Id = "s2", FirstName = "Old", Email = "old@mail.com" } };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateStudentDto { FirstName = "New", LastName = "Name", Email = "new@mail.com", DateOfBirth = DateTime.Today };

            var result = await _service.UpdateStudentAsync("s2", updateDto);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal("New", _context.Students.First().AppUser.FirstName);
            Assert.Equal("new@mail.com", _context.Students.First().AppUser.Email);
        }

        [Fact]
        public async Task DeleteStudentAsync_ShouldDeleteStudent()
        {
            var appUser = new AppUser { Id = "s3", FirstName = "ToDelete" };
            var student = new SMS.Core.Models.Entities.Student { Id = 4, AppUserId = "s3", AppUser = appUser };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(u => u.DeleteAsync(appUser)).ReturnsAsync(IdentityResult.Success);

            var result = await _service.DeleteStudentAsync("s3");

            Assert.Equal(200, result.StatusCode);
            Assert.Empty(_context.Students);
        }
    }

}
