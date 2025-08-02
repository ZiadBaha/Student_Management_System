using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using SMS.Core.Models.Account.AddUser;
using SMS.Core.Models.Entities;
using SMS.Core.Models.Identity;
using SMS.Core.Services;
using SMS.Repository.Data.Context;
using SMS.Repository.Services;
using SMS.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SMS.Tests.ServicesTests
{
    public class AdminServiceTests
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvMock;
        private readonly Mock<ILogger<AdminService>> _loggerMock;
        private readonly StoreContext _dbContext;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _userManagerMock = MocksHelper.GetMockUserManager();
            _roleManagerMock = MocksHelper.GetMockRoleManager();
            _mapperMock = new Mock<IMapper>();
            _emailServiceMock = new Mock<IEmailService>();
            _webHostEnvMock = new Mock<IWebHostEnvironment>();
            _loggerMock = new Mock<ILogger<AdminService>>();

            var dbOptions = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new StoreContext(dbOptions);

            _adminService = new AdminService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _emailServiceMock.Object,
                _webHostEnvMock.Object,
                _loggerMock.Object,
                _dbContext
            );
        }

        [Fact]
        public async Task AddStudentAsync_ShouldReturnError_WhenPasswordsDoNotMatch()
        {
            var request = new AddStudentRequest
            {
                Email = "test@student.com",
                Password = "Pass@1234",
                ConfirmPassword = "Wrong123"
            };

            var result = await _adminService.AddStudentAsync(request);

            Assert.Equal(400, result.StatusCode);
            Assert.Contains("do not match", result.Message!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AddStudentAsync_ShouldReturnError_WhenUserAlreadyExists()
        {
            var request = new AddStudentRequest
            {
                Email = "existing@student.com",
                Password = "Pass@1234",
                ConfirmPassword = "Pass@1234"
            };

            _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync(new AppUser());

            var result = await _adminService.AddStudentAsync(request);

            Assert.Equal(400, result.StatusCode);
            Assert.Contains("already exists", result.Message!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AddStudentAsync_ShouldReturnError_WhenRoleDoesNotExist()
        {
            var request = new AddStudentRequest
            {
                Email = "ziad@student.com",
                Password = "Pass@1234",
                ConfirmPassword = "Pass@1234"
            };

            _mapperMock.Setup(m => m.Map<AppUser>(request)).Returns(new AppUser { Email = request.Email });
            _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<AppUser>(), request.Password)).ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(r => r.RoleExistsAsync("Student")).ReturnsAsync(false);

            var result = await _adminService.AddStudentAsync(request);

            Assert.Equal(404, result.StatusCode);
            Assert.Contains("role", result.Message!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AddStudentAsync_ShouldReturnError_WhenCreateFails()
        {
            var request = new AddStudentRequest
            {
                Email = "fail@student.com",
                Password = "Pass@1234",
                ConfirmPassword = "Pass@1234"
            };

            _mapperMock.Setup(m => m.Map<AppUser>(request)).Returns(new AppUser { Email = request.Email });
            _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<AppUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error creating user" }));

            var result = await _adminService.AddStudentAsync(request);

            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Error creating user", result.Message!);
        }

      
        [Fact]
        public async Task AddTeacherAsync_ShouldReturnError_WhenAddToRoleFails()
        {
            var request = new AddTeacherRequest
            {
                FirstName = "ziad",
                LastName = "bahaa",
                Email = "teacher@test.com",
                Password = "Pass@1234",
                ConfirmPassword = "Pass@1234",
                DepartmentId = 1
            };

            await _dbContext.Departments.AddAsync(new Department
            {
                Id = 1,
                Name = "Math"
            });
            await _dbContext.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<AppUser>(request)).Returns(new AppUser { Email = request.Email });
            _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<AppUser>(), request.Password)).ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(r => r.RoleExistsAsync("Teacher")).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<AppUser>(), "Teacher"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Add to role failed" }));

            var result = await _adminService.AddTeacherAsync(request);

            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Add to role failed", result.Message!);
        }
    }
}



