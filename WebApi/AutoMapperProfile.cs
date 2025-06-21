using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Models;

namespace WebApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<EmployeeRequest, Employee>();
            CreateMap<Employee, EmployeeDTO>();
        }
    }
}
