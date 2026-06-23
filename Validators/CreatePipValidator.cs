using FluentValidation;
using HRMS.Api.DTOs.PIPDtos;

namespace HRMS.Api.Validators
{
    public class CreatePipValidator : AbstractValidator<CreatePipDto>
    {
        public CreatePipValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0);
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.Status).MaximumLength(50);
        }
    }
}
