using FluentValidation;
using HRMS.Api.DTOs.EmployeeDtos;

namespace HRMS.Api.Validators
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
    {
        public UpdateEmployeeValidator()
        {
            RuleFor(x => x.EmployeeCode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
            RuleFor(x => x.EmploymentTypeId).NotEqual(Guid.Empty);
            RuleFor(x => x.LocationId).NotEqual(Guid.Empty);
            RuleFor(x => x.WorkModelId).NotEqual(Guid.Empty);
            RuleFor(x => x.PracticeId).NotEqual(Guid.Empty);
            RuleFor(x => x.DepartmentTypeId).NotEqual(Guid.Empty);
            RuleFor(x => x.StatusId).NotEqual(Guid.Empty);
        }
    }
}
