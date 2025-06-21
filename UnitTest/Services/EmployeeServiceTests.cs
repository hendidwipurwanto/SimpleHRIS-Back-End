using Application.Services;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Services
{
    public class EmployeeServiceTests
    {
        private readonly EmployeeService _employeeService;
        private readonly Mock<IEmployeeRepository> _repoMock;
        private readonly IMapper _mapper;

        public EmployeeServiceTests()
        {
            _repoMock = new Mock<IEmployeeRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EmployeeRequest, Employee>();
                cfg.CreateMap<Employee, EmployeeDTO>();
            });
            _mapper = config.CreateMapper();

            _employeeService = new EmployeeService(_repoMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ReturnsList()
        {
            var employees = new List<Employee>
            {
                new Employee { Id = 1, NIK = "001", FullName = "Budi", Position = "Manager", Department = "HR", Email = "budi@test.com" }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);

            var result = await _employeeService.GetAllEmployeesAsync();

            Assert.True(result.Status);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("Budi", result.Data[0].FullName);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_Found()
        {
            var employee = new Employee { Id = 1, NIK = "001", FullName = "Budi", Position = "Manager", Department = "HR", Email = "budi@test.com" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);

            var result = await _employeeService.GetEmployeeByIdAsync(1);

            Assert.True(result.Status);
            Assert.Equal("Budi", result.Data.FullName);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Employee)null);

            var result = await _employeeService.GetEmployeeByIdAsync(2);

            Assert.False(result.Status);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateEmployeeAsync_Success()
        {
            Employee capturedEmployee = null;
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Employee>())).Callback<Employee>(e => capturedEmployee = e).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var request = new EmployeeRequest
            {
                NIK = "002",
                FullName = "Sari",
                Position = "Staff",
                Department = "IT",
                Email = "sari@test.com"
            };

            var result = await _employeeService.CreateEmployeeAsync(request);

            Assert.True(result.Status);
            Assert.Equal("Sari", capturedEmployee.FullName);
            Assert.Equal("Sari", result.Data.FullName);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_Success()
        {
            var employee = new Employee { Id = 1, NIK = "001", FullName = "Budi", Position = "Manager", Department = "HR", Email = "budi@test.com" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _repoMock.Setup(r => r.Update(It.IsAny<Employee>()));
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var request = new EmployeeRequest
            {
                NIK = "001",
                FullName = "Budi Updated",
                Position = "Manager",
                Department = "HR",
                Email = "budi.updated@test.com"
            };

            var result = await _employeeService.UpdateEmployeeAsync(1, request);

            Assert.True(result.Status);
            Assert.Equal("Budi Updated", employee.FullName);
            Assert.Equal("Budi Updated", result.Data.FullName);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Employee)null);

            var request = new EmployeeRequest
            {
                NIK = "002",
                FullName = "Sari",
                Position = "Staff",
                Department = "IT",
                Email = "sari@test.com"
            };

            var result = await _employeeService.UpdateEmployeeAsync(2, request);

            Assert.False(result.Status);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_Success()
        {
            var employee = new Employee { Id = 1 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _repoMock.Setup(r => r.Delete(employee));
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _employeeService.DeleteEmployeeAsync(1);

            Assert.True(result.Status);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Employee)null);

            var result = await _employeeService.DeleteEmployeeAsync(2);

            Assert.False(result.Status);
            Assert.False(result.Data);
        }
    }
}
