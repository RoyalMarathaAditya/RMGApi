using FluentValidation;
using HRMS.Api.DTOs.ProjectAllocationDtos;

namespace HRMS.Api.Validators
{
    public class CreateAllocationValidator : AbstractValidator<CreateAllocationDto>
    {
        public CreateAllocationValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0);
            RuleFor(x => x.ProjectId).GreaterThan(0);
            RuleFor(x => x.EmployeeProjectStatusId).NotEqual(Guid.Empty);
            RuleFor(x => x.AllocationStatusId).NotEqual(Guid.Empty);
            RuleFor(x => x.AllocationStartDate).NotEmpty();
            RuleFor(x => x.AllocationPercentage).InclusiveBetween(0, 100);
            RuleFor(x => x.BillablePercentage).InclusiveBetween(0, 100).When(x => x.BillablePercentage.HasValue);
        }
    }
}
