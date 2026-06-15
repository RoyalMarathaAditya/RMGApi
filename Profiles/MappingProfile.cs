using AutoMapper;
using HRMS.Api.DTOs;
using HRMS.Api.Models;

namespace HRMS.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Employee Mappings
            CreateMap<Employee, EmployeeDto>();
            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.DateOfJoining, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateEmployeeDto, Employee>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateOfJoining, opt => opt.Ignore());

            // User Mappings
            CreateMap<User, UserDto>();

            // Client Mappings
            CreateMap<Client, ClientDto>();

            // Skill Mappings
            CreateMap<Skill, SkillDto>();

            // ProjectDetails Mappings
            CreateMap<ProjectDetails, ProjectDetailsDto>();
        }
    }
}
