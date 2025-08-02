

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SMS.Core.DTOs.Department;
using SMS.Core.Models.Entities;
using SMS.Repository.Data.Context;
using SMS.Repository.Services;
using Xunit;

namespace SMS.Tests.ServicesTests
{
    public class DepartmentServiceTests
    {
        private readonly StoreContext _dbContext;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<DepartmentService>> _loggerMock;
        private readonly DepartmentService _departmentService;

        public DepartmentServiceTests()
        {
            var dbOptions = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new StoreContext(dbOptions);

            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<DepartmentService>>();

            _departmentService = new DepartmentService(_dbContext, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllDepartmentsAsync_ShouldReturnDepartments()
        {
            await _dbContext.Departments.AddAsync(new Department { Name = "Math" });
            await _dbContext.Departments.AddAsync(new Department { Name = "Science" });
            await _dbContext.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<List<DepartmentDto>>(It.IsAny<List<Department>>()))
                       .Returns(new List<DepartmentDto>
                       {
                           new DepartmentDto { Id = 1, Name = "Math" },
                           new DepartmentDto { Id = 2, Name = "Science" }
                       });

            var result = await _departmentService.GetAllDepartmentsAsync();

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, result.Data!.Count);
        }

        [Fact]
        public async Task CreateDepartmentAsync_ShouldCreateDepartment_WhenNameIsUnique()
        {
            var dto = new CreateDepartmentDto { Name = "Physics" };

            _mapperMock.Setup(m => m.Map<Department>(dto)).Returns(new Department { Name = dto.Name });

            var result = await _departmentService.CreateDepartmentAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.Contains("created", result.Message!);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_ShouldUpdateDepartment_WhenValid()
        {
            var existing = new Department { Name = "Old", Id = 1 };
            _dbContext.Departments.Add(existing);
            await _dbContext.SaveChangesAsync();

            var dto = new UpdateDepartmentDto { Id = 1, Name = "Updated" };

            _mapperMock.Setup(m => m.Map(dto, existing)).Callback(() => existing.Name = dto.Name);

            var result = await _departmentService.UpdateDepartmentAsync(dto);

            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteDepartmentAsync_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var result = await _departmentService.DeleteDepartmentAsync(999);

            Assert.Equal(404, result.StatusCode);
            Assert.Contains("not found", result.Message!);
        }

        [Fact]
        public async Task DeleteDepartmentAsync_ShouldDelete_WhenExists()
        {
            var department = new Department { Id = 1, Name = "ToDelete" };
            _dbContext.Departments.Add(department);
            await _dbContext.SaveChangesAsync();

            var result = await _departmentService.DeleteDepartmentAsync(department.Id);

            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);
        }


    }
}
