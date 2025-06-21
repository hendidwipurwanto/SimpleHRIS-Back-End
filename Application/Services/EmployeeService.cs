using Application.Interfaces;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<GenericResponse<List<EmployeeDTO>>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            var employeeDTOs = _mapper.Map<List<EmployeeDTO>>(employees);
            return new GenericResponse<List<EmployeeDTO>>
            {
                Status = true,
                Message = "Success",
                Data = employeeDTOs
            };
        }

        public async Task<GenericResponse<EmployeeDTO>> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return new GenericResponse<EmployeeDTO> { Status = false, Message = "Employee not found" };

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
            return new GenericResponse<EmployeeDTO> { Status = true, Data = employeeDTO };
        }

        public async Task<GenericResponse<EmployeeDTO>> CreateEmployeeAsync(EmployeeRequest request)
        {
            var employee = _mapper.Map<Employee>(request);
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
            return new GenericResponse<EmployeeDTO> { Status = true, Data = employeeDTO, Message = "Employee created" };
        }

        public async Task<GenericResponse<EmployeeDTO>> UpdateEmployeeAsync(int id, EmployeeRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return new GenericResponse<EmployeeDTO> { Status = false, Message = "Employee not found" };

            _mapper.Map(request, employee);

            _employeeRepository.Update(employee);
            await _employeeRepository.SaveChangesAsync();

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
            return new GenericResponse<EmployeeDTO> { Status = true, Data = employeeDTO, Message = "Employee updated" };
        }

        public async Task<GenericResponse<bool>> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return new GenericResponse<bool> { Status = false, Message = "Employee not found", Data = false };

            _employeeRepository.Delete(employee);
            await _employeeRepository.SaveChangesAsync();

            return new GenericResponse<bool> { Status = true, Message = "Employee deleted", Data = true };
        }
    }
}
