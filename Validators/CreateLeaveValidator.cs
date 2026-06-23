using FluentValidation;
using HRMS.Api.DTOs.LeaveDtos;

namespace HRMS.Api.Validators
{
    public class CreateLeaveValidator : AbstractValidator<CreateLeaveDto>
    {
        public CreateLeaveValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0);
            RuleFor(x => x.LeaveTypeId).NotEmpty();
            RuleFor(x => x.FromDate).NotEmpty();
            RuleFor(x => x.ToDate).NotEmpty();
            RuleFor(x => x.NumberOfDays).GreaterThan(0);
            RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate);
        }
    }
}
