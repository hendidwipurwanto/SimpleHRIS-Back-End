using Domain.DTOs;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmployeeService
    {
            Task<GenericResponse<List<EmployeeDTO>>> GetAllEmployeesAsync();
            Task<GenericResponse<EmployeeDTO>> GetEmployeeByIdAsync(int id);
            Task<GenericResponse<EmployeeDTO>> CreateEmployeeAsync(EmployeeRequest request);
            Task<GenericResponse<EmployeeDTO>> UpdateEmployeeAsync(int id, EmployeeRequest request);
            Task<GenericResponse<bool>> DeleteEmployeeAsync(int id);        
    }
}
