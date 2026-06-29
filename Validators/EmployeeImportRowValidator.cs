using FluentValidation;
using HRMS.Api.DTOs.EmployeeDtos;

namespace HRMS.Api.Validators
{
    public class EmployeeImportRowValidator : AbstractValidator<EmployeeImportRowDto>
    {
        public EmployeeImportRowValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Designation)
                .NotEmpty().WithMessage("Role is required.")
                .MaximumLength(100);

            RuleFor(x => x.Practice)
                .NotEmpty().WithMessage("Practice is required.")
                .MaximumLength(100);

            RuleFor(x => x.EmployeeType)
                .NotEmpty().WithMessage("FTE/ Consultant is required.")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150);

            RuleFor(x => x.DOJ)
                .NotNull().WithMessage("Date of Joining is required.");
        }
    }
}
