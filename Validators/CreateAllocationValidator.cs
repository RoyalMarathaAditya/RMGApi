using FluentValidation;
using HRMS.Api.DTOs.AllocationDtos;

namespace HRMS.Api.Validators
{
    public class CreateAllocationValidator : AbstractValidator<CreateAllocationDto>
    {
        public CreateAllocationValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0);
            RuleFor(x => x.ProjectId).GreaterThan(0);
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).When(x => x.EndDate.HasValue);
            RuleFor(x => x.AllocationPercentage).InclusiveBetween(1, 100);
            RuleFor(x => x.AllocationStatus).MaximumLength(50);
            RuleFor(x => x.Notes).MaximumLength(1000);
        }
    }

    public class UpdateAllocationValidator : AbstractValidator<UpdateAllocationDto>
    {
        public UpdateAllocationValidator()
        {
            RuleFor(x => x.AllocationPercentage).InclusiveBetween(1, 100).When(x => x.AllocationPercentage.HasValue);
            RuleFor(x => x.AllocationStatus).MaximumLength(50);
            RuleFor(x => x.Notes).MaximumLength(1000);
        }
    }
}
