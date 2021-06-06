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
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Tên không được để trống")
                .MaximumLength(200).WithMessage("First name cannot be over 200 character");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Tên không được để trống")
                .MaximumLength(200).WithMessage("Last name cannot be over 200 character");
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Tên không được để trống")
                .Length(6, 14).WithMessage("Username cannot be blank");
            RuleFor(x => x.Phone)
                .Matches("^((84|0)[1|3|5|7|8|9])+([0-9]{8})$").WithMessage("Số điện thoại không hợp lệ");
            RuleFor(x => x.Email)
                .Matches("^(([^<>()\\[\\]\\.,;:\\s@]+(\\.[^<>()\\[\\]\\.,;:\\s@]+)*)|(.+))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$")
                .WithMessage("Email không hợp lệ");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .Length(8, 50).WithMessage("Mật khẩu phải có ít nhất 8 kí tự");
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Bạn chưa xác nhận mật khẩu")
                .Matches(x => x.Password)
                .WithMessage("Mật khẩu không khớp");
        }
    }
}