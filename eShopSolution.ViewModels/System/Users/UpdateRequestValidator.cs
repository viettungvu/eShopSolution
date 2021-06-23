using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class UpdateRequestValidator : AbstractValidator<UserUpdateRequest>
    {
        public UpdateRequestValidator()
        {
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name should not be blank")
            .MaximumLength(200).WithMessage("First name should not be over 200 character");
            RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Lastname should not be blank")
                    .MaximumLength(200).WithMessage("Last name should not be over 200 character");
            RuleFor(x => x.Phone)
                    .Matches("^((84|0)[1|3|5|7|8|9])+([0-9]{8})$").WithMessage("Phone number is invalid");
            RuleFor(x => x.Email)
                    .Matches("^(([^<>()\\[\\]\\.,;:\\s@]+(\\.[^<>()\\[\\]\\.,;:\\s@]+)*)|(.+))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$")
                    .WithMessage("Email is invalid");
        }
    }
}