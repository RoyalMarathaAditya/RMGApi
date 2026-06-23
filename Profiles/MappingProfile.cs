using AutoMapper;
using HRMS.Api.DTOs;
using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.DTOs.MasterDtos;
using HRMS.Api.DTOs.ProjectAllocationDtos;
using HRMS.Api.DTOs.LeaveDtos;
using HRMS.Api.DTOs.PIPDtos;
using HRMS.Api.Models;

namespace HRMS.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Employee
            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.EmployeeSkills, opt => opt.Ignore());

            CreateMap<UpdateEmployeeDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeSkills, opt => opt.Ignore());

            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.EmploymentType, opt => opt.MapFrom(src => src.EmploymentType != null ? src.EmploymentType.Name : null))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location != null ? src.Location.Name : null))
                .ForMember(dest => dest.WorkModel, opt => opt.MapFrom(src => src.WorkModel != null ? src.WorkModel.Name : null))
                .ForMember(dest => dest.Practice, opt => opt.MapFrom(src => src.Practice != null ? src.Practice.Name : null))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.DepartmentType != null ? src.DepartmentType.Name : null))
                .ForMember(dest => dest.EmployeeStatus, opt => opt.MapFrom(src => src.EmployeeStatus != null ? src.EmployeeStatus.Name : null))
                .ForMember(dest => dest.ReportingManagerName, opt => opt.MapFrom(src => src.ReportingManager != null ? src.ReportingManager.FullName : null))
                .ForMember(dest => dest.PracticeHeadName, opt => opt.MapFrom(src => src.PracticeHead != null ? src.PracticeHead.FullName : null))
                .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.Designation != null ? src.Designation.Name : null))
                .ForMember(dest => dest.DesignationId, opt => opt.MapFrom(src => src.DesignationId))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.EmployeeSkills.Select(es => new MasterDto { Id = es.SkillId, Name = es.Skill != null ? es.Skill.Name : "" }).ToList()))
                .ForMember(dest => dest.EmploymentTypeId, opt => opt.MapFrom(src => src.EmploymentTypeId))
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId))
                .ForMember(dest => dest.WorkModelId, opt => opt.MapFrom(src => src.WorkModelId))
                .ForMember(dest => dest.PracticeId, opt => opt.MapFrom(src => src.PracticeId))
                .ForMember(dest => dest.DepartmentTypeId, opt => opt.MapFrom(src => src.DepartmentTypeId))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId));
            #endregion

            #region Master
            CreateMap<RoleMaster, MasterDto>();
            CreateMap<EmploymentTypeMaster, MasterDto>();
            CreateMap<StatusMaster, MasterDto>();
            CreateMap<WorkModelMaster, MasterDto>();
            CreateMap<LeaveTypeMaster, MasterDto>();
            CreateMap<PricingTypeMaster, MasterDto>();
            CreateMap<ProjectTypeMaster, MasterDto>();
            CreateMap<AllocationStatusMaster, MasterDto>();
            CreateMap<EmployeeProjectStatusMaster, MasterDto>();
            CreateMap<DepartmentTypeMaster, MasterDto>();
            CreateMap<DesignationMaster, MasterDto>();
            CreateMap<Location, MasterDto>();
            CreateMap<Practice, MasterDto>();
            CreateMap<Skill, MasterDto>();
            #endregion

            #region Allocation
            CreateMap<CreateAllocationDto, ProjectAllocation>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateAllocationDto, ProjectAllocation>()
                .ForMember(dest => dest.ModifiedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ProjectAllocation, AllocationDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
                .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.EmployeeCode : null))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectName : null))
                .ForMember(dest => dest.EmployeeProjectStatus, opt => opt.MapFrom(src => src.EmployeeProjectStatus != null ? src.EmployeeProjectStatus.Name : null))
                .ForMember(dest => dest.AllocationStatus, opt => opt.MapFrom(src => src.AllocationStatus != null ? src.AllocationStatus.Name : null));
            #endregion

            #region Leave
            CreateMap<CreateLeaveDto, EmployeeLeave>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<EmployeeLeave, LeaveDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType != null ? src.LeaveType.Name : null));
            #endregion

            #region PIP
            CreateMap<CreatePipDto, PIP>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<PIP, PipDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null));
            #endregion

            #region Designation
            CreateMap<DesignationMaster, DesignationDto>();
            #endregion

            #region Existing
            CreateMap<User, UserDto>();
            CreateMap<Client, ClientDto>();
            #endregion
        }
    }
}
