using Application.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Controllers;

namespace UnitTest.Controllers
{
    public class EmployeeControllerTests
    {
        private readonly EmployeeController _controller;
        private readonly Mock<IEmployeeService> _serviceMock;

        public EmployeeControllerTests()
        {
            _serviceMock = new Mock<IEmployeeService>();
            _controller = new EmployeeController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithEmployeeList()
        {
            var fakeResponse = new GenericResponse<List<EmployeeDTO>>
            {
                Status = true,
                Data = new List<EmployeeDTO>
                {
                    new EmployeeDTO { Id = 1, NIK = "001", FullName = "Budi", Position = "Manager", Department = "HR", Email = "budi@test.com" }
                }
            };

            _serviceMock.Setup(s => s.GetAllEmployeesAsync()).ReturnsAsync(fakeResponse);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponse<List<EmployeeDTO>>>(okResult.Value);
            Assert.Single(returnValue.Data);
            Assert.Equal("Budi", returnValue.Data[0].FullName);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenFound()
        {
            var fakeResponse = new GenericResponse<EmployeeDTO>
            {
                Status = true,
                Data = new EmployeeDTO { Id = 1, NIK = "001", FullName = "Budi" }
            };

            _serviceMock.Setup(s => s.GetEmployeeByIdAsync(1)).ReturnsAsync(fakeResponse);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<GenericResponse<EmployeeDTO>>(okResult.Value);
            Assert.Equal(1, data.Data.Id);
            Assert.Equal("Budi", data.Data.FullName);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotFound()
        {
            var fakeResponse = new GenericResponse<EmployeeDTO>
            {
                Status = false,
                Message = "Employee not found"
            };

            _serviceMock.Setup(s => s.GetEmployeeByIdAsync(2)).ReturnsAsync(fakeResponse);

            var result = await _controller.GetById(2);

            var nfResult = Assert.IsType<NotFoundObjectResult>(result);
            var data = Assert.IsType<GenericResponse<EmployeeDTO>>(nfResult.Value);
            Assert.False(data.Status);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenSuccess()
        {
            var inputRequest = new EmployeeRequest
            {
                NIK = "002",
                FullName = "Sari",
                Position = "Staff",
                Department = "IT",
                Email = "sari@test.com"
            };

            var fakeResponse = new GenericResponse<EmployeeDTO>
            {
                Status = true,
                Data = new EmployeeDTO { Id = 2, FullName = "Sari" }
            };

            _serviceMock.Setup(s => s.CreateEmployeeAsync(inputRequest)).ReturnsAsync(fakeResponse);

            var result = await _controller.Create(inputRequest);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var data = Assert.IsType<GenericResponse<EmployeeDTO>>(createdResult.Value);
            Assert.Equal("Sari", data.Data.FullName);
            Assert.Equal(2, data.Data.Id);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenFailed()
        {
            var inputRequest = new EmployeeRequest
            {
                NIK = "002",
                FullName = "Sari"
            };

            var fakeResponse = new GenericResponse<EmployeeDTO>
            {
                Status = false,
                Message = "Validation failed"
            };

            _serviceMock.Setup(s => s.CreateEmployeeAsync(inputRequest)).ReturnsAsync(fakeResponse);

            var result = await _controller.Create(inputRequest);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var data = Assert.IsType<GenericResponse<EmployeeDTO>>(badRequest.Value);
            Assert.False(data.Status);
            Assert.Equal("Validation failed", data.Message);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            var inputRequest = new EmployeeRequest
            {
                NIK = "001",
                FullName = "Budi Updated",
                Position = "Manager",
                Department = "HR",
                Email = "budi.updated@test.com"
            };

            var fakeResponse = new GenericResponse<EmployeeDTO>
            {
                Status = true,
                Data = new EmployeeDTO { Id = 1, FullName = "Budi Updated" }
            };

            _serviceMock.Setup(s => s.UpdateEmployeeAsync(1, inputRequest)).ReturnsAsync(fakeResponse);

            var result = await _controller.Update(1, inputRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<GenericResponse<EmployeeDTO>>(okResult.Value);
            Assert.Equal("Budi Updated", data.Data.FullName);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenNotFound()
        {
            var inputRequest = new EmployeeRequest
            {
                NIK = "002",
                FullName = "Sari",
                Position = "Staff",
                Department = "IT",
                Email = "sari@test.com"
            };

            var fakeResponse = new GenericResponse<EmployeeDTO>
            {
                Status = false,
                Message = "Employee not found"
            };

            _serviceMock.Setup(s => s.UpdateEmployeeAsync(2, inputRequest)).ReturnsAsync(fakeResponse);

            var result = await _controller.Update(2, inputRequest);

            var nfResult = Assert.IsType<NotFoundObjectResult>(result);
            var data = Assert.IsType<GenericResponse<EmployeeDTO>>(nfResult.Value);
            Assert.False(data.Status);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            var fakeResponse = new GenericResponse<bool>
            {
                Status = true,
                Data = true,
                Message = "Deleted"
            };

            _serviceMock.Setup(s => s.DeleteEmployeeAsync(1)).ReturnsAsync(fakeResponse);

            var result = await _controller.Delete(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<GenericResponse<bool>>(okResult.Value);
            Assert.True(data.Data);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenNotFound()
        {
            var fakeResponse = new GenericResponse<bool>
            {
                Status = false,
                Data = false,
                Message = "Employee not found"
            };

            _serviceMock.Setup(s => s.DeleteEmployeeAsync(2)).ReturnsAsync(fakeResponse);

            var result = await _controller.Delete(2);

            var nfResult = Assert.IsType<NotFoundObjectResult>(result);
            var data = Assert.IsType<GenericResponse<bool>>(nfResult.Value);
            Assert.False(data.Data);
        }
    }
}
