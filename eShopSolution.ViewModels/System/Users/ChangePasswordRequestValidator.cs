using FluentValidation;

namespace eShopSolution.ViewModels.System.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password should not be blank")
                .Matches(@"[0-9]+").WithMessage("Password should contain at least one lower case letter")
                .Matches(@"[A-Z]+").WithMessage("Password should contain at least one upper case letter")
                .Matches(@".{8,15}").WithMessage("Password should not be lesser than 8 or greater than 15 characters")
                .Matches(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]").WithMessage("Password should contain at least one special case character");
            RuleFor(x => x.ConfirmPassword)
                .Matches(x => x.NewPassword).WithMessage("Password does not match");
        }
    }
}