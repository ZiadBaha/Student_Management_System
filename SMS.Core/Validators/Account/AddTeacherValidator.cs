using FluentValidation;
using SMS.Core.Models.Account.AddUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Validators.Account
{
    public class AddTeacherValidator : AbstractValidator<AddTeacherRequest>
    {
        public AddTeacherValidator()
        {
            Include(new RegisterValidator());

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department is required.");
        }
    }
}
