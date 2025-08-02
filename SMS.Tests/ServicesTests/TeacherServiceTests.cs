using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SMS.Core.DTOs.Teacher;
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
    public class TeacherServiceTests
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<ILogger<TeacherService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly StoreContext _context;
        private readonly TeacherService _service;

        public TeacherServiceTests()
        {
            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new StoreContext(options);
            _loggerMock = new Mock<ILogger<TeacherService>>();
            _mapperMock = new Mock<IMapper>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object, null, null, null, null, null, null, null, null);

            _service = new TeacherService(_userManagerMock.Object, _loggerMock.Object, _mapperMock.Object, _context);
        }

        [Fact]
        public async Task GetAllTeachersAsync_ShouldReturnAllTeachers()
        {
            _context.Teachers.Add(new Teacher { Id = 1, AppUserId = "1", AppUser = new AppUser { Id = "1", FirstName = "John" } });
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<List<GetAllTeacherDto>>(It.IsAny<List<Teacher>>()))
                .Returns(new List<GetAllTeacherDto> { new GetAllTeacherDto { TeacherId = "1", FirstName = "John" } });

            var result = await _service.GetAllTeachersAsync();

            Assert.Equal(200, result.StatusCode);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetTeacherByIdAsync_ShouldReturnTeacher_WhenFound()
        {
            _context.Teachers.Add(new Teacher { Id = 2, AppUserId = "2", AppUser = new AppUser { Id = "2", FirstName = "Jane" } });
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<GetTeacherByIdDto>(It.IsAny<Teacher>()))
                .Returns(new GetTeacherByIdDto { TeacherId = "2", FirstName = "Jane" });

            var result = await _service.GetTeacherByIdAsync("2");

            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Jane", result.Data.FirstName);
        }

        [Fact]
        public async Task GetTeacherByIdAsync_ShouldReturnNotFound_WhenNotFound()
        {
            var result = await _service.GetTeacherByIdAsync("not_exist");

            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task UpdateTeacherAsync_ShouldUpdateAndReturnSuccess()
        {
            var appUser = new AppUser { Id = "3", FirstName = "Old", LastName = "Name", Email = "old@mail.com" };
            _context.Users.Add(appUser);
            _context.Teachers.Add(new Teacher { Id = 3, AppUserId = "3", AppUser = appUser, DepartmentId = 1 });
            await _context.SaveChangesAsync();

            var updateDto = new UpdateTeacherDto { FirstName = "New", LastName = "Name", Email = "new@mail.com", DepartmentId = 2 };

            var result = await _service.UpdateTeacherAsync("3", updateDto);

            Assert.Equal(200, result.StatusCode);
            Assert.Equal("New", _context.Users.First(u => u.Id == "3").FirstName);
        }

        [Fact]
        public async Task DeleteTeacherAsync_ShouldDeleteTeacher_WhenExists()
        {
            var user = new AppUser { Id = "4", FirstName = "Delete", LastName = "Me", Email = "del@mail.com" };
            _context.Users.Add(user);
            _context.Teachers.Add(new Teacher { Id = 4, AppUserId = "4", AppUser = user });
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _service.DeleteTeacherAsync("4");

            Assert.Equal(200, result.StatusCode);
            Assert.False(_context.Teachers.Any(t => t.AppUserId == "4"));
        }
    }

}
