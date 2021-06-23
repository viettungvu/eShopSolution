using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace eShopSolution.ViewModels.System.Users
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name should not be blank")
            .MaximumLength(200).WithMessage("First name should not be over 200 character");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Lastname should not be blank")
                .MaximumLength(200).WithMessage("Last name should not be over 200 character");
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username should not be blank")
                .Length(6, 14).WithMessage("Username must be between 6 to 14 characters");
            RuleFor(x => x.Phone)
                .Matches("^((84|0)[1|3|5|7|8|9])+([0-9]{8})$").WithMessage("Phone number is invalid");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email should not be blank")
                .Matches("^(([^<>()\\[\\]\\.,;:\\s@]+(\\.[^<>()\\[\\]\\.,;:\\s@]+)*)|(.+))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$")
                .WithMessage("Email is invalid");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password should not be blank")
                .MinimumLength(8).WithMessage("Password is at least 8 charaters");
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("You haven't been confirm password")
                .Matches(x => x.Password)
                .WithMessage("Password does not match");
        }
    }
}