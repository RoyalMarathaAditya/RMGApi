using AutoMapper;
using HRMS.Api.DTOs;
using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.DTOs.DesignationDtos;
using HRMS.Api.Models;

namespace HRMS.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
             // Create Employee
            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Update Employee
            CreateMap<UpdateEmployeeDto, Employee>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Employee → EmployeeDto (FULL RESPONSE)
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Designation,
                    opt => opt.MapFrom(src => src.Designation != null ? src.Designation.Name : null))

                .ForMember(dest => dest.Practice,
                    opt => opt.MapFrom(src => src.Practice != null ? src.Practice.Name : null))

                .ForMember(dest => dest.SubPractice,
                    opt => opt.MapFrom(src => src.SubPractice != null ? src.SubPractice.Name : null))

                .ForMember(dest => dest.Location,
                    opt => opt.MapFrom(src => src.Location != null ? src.Location.Name : null))

                .ForMember(dest => dest.ManagerName,
                    opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null));

       

            CreateMap<Designation, DesignationDto>();
            CreateMap<CreateDesignationDto, Designation>();
            CreateMap<UpdateDesignationDto, Designation>();

            CreateMap<User, UserDto>();
            CreateMap<Client, ClientDto>();
            CreateMap<Skill, SkillDto>();
            CreateMap<Project, ProjectDto>();
        }
    }
}