using FluentValidation;
using HRMS.Api.DTOs.ResourceRequestDtos;

namespace HRMS.Api.Validators
{
    public class CreateResourceRequestValidator : AbstractValidator<CreateResourceRequestDto>
    {
        public CreateResourceRequestValidator()
        {
            RuleFor(x => x.ProjectId).GreaterThan(0);
            RuleFor(x => x.RequiredCount).GreaterThan(0);
            RuleFor(x => x.RequiredByDate).NotEmpty();
            RuleFor(x => x.RequiredByDate).GreaterThan(DateTime.UtcNow).When(x => x.RequiredByDate != default);
            RuleFor(x => x.Notes).MaximumLength(1000);
        }
    }
}
