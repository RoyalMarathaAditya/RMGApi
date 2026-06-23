using FluentValidation;
using HRMS.Api.DTOs.EmployeeDtos;

namespace HRMS.Api.Validators
{
    public class EmployeeImportRowValidator : AbstractValidator<EmployeeImportRowDto>
    {
        public EmployeeImportRowValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Designation)
                .NotEmpty().WithMessage("Designation is required.")
                .MaximumLength(100);

            RuleFor(x => x.Practice)
                .NotEmpty().WithMessage("Practice is required.")
                .MaximumLength(100);

            RuleFor(x => x.EmployeeType)
                .NotEmpty().WithMessage("Employee Type is required.")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150);

            RuleFor(x => x.DOJ)
                .NotNull().WithMessage("Date of Joining is required.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\d{10,15}$").WithMessage("Phone number must be numeric and 10-15 digits.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Experience)
                .GreaterThanOrEqualTo(0).WithMessage("Experience must be greater than or equal to 0.")
                .When(x => x.Experience.HasValue);
        }
    }
}
