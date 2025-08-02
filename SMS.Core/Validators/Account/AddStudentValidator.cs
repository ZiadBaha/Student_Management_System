using FluentValidation;
using SMS.Core.Models.Account.AddUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Validators.Account
{
    public class AddStudentValidator : AbstractValidator<AddStudentRequest>
    {
        public AddStudentValidator()
        {
            Include(new RegisterValidator());

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .LessThan(DateTime.Today)
                .WithMessage("Date of birth must be in the past.");
        }
    }
}
