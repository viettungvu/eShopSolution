using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        /// <summary>
        /// Lớp này kế thừa abstractclass: AbstractValidator để thực hiện validate cho LoginRequest
        /// Trong đó có method RuleFor dùng để định nghĩa ra các rule validate cho từng property trong request
        /// </summary>
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username should not be blank");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password should not be blank")
               .MinimumLength(8).WithMessage("Password is at least 8 characters");
        }
    }
}